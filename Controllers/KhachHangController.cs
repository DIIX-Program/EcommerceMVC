using AutoMapper;
using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using EcommerceMVC.Models;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceMVC.Controllers
{
	public class KhachHangController : Controller
	{
		private readonly Hshop2023Context db;
		private readonly IMapper _mapper;
		private readonly IVNMailService _mailService;

		public KhachHangController(Hshop2023Context context, IMapper mapper, IVNMailService mailService)
		{
			db = context;
			_mapper = mapper;
			_mailService = mailService;
		}

		#region Register
		[HttpGet]
		public IActionResult DangKy()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> DangKy(RegisterVM model, IFormFile? Hinh)
		{
			if (ModelState.IsValid)
			{
				if (db.KhachHangs.Any(kh => kh.MaKh == model.MaKh))
				{
					ModelState.AddModelError("MaKh", "Tên đăng nhập đã tồn tại.");
					return View();
				}

				if (db.KhachHangs.Any(kh => kh.Email == model.Email))
				{
					ModelState.AddModelError("Email", "Email đã được sử dụng.");
					return View();
				}

				try
				{
					var khachHang = _mapper.Map<KhachHang>(model);
					var rd = new Random();
					var verificationCode = rd.Next(100000, 999999).ToString();
					khachHang.RandomKey = verificationCode;
					khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
					khachHang.HieuLuc = false;// Cần xác thực qua mail
					khachHang.VaiTro = 0;
					khachHang.NgaySinh = DateTime.Now; // Sửa lỗi SqlDateTimeOverflowException

					if (Hinh != null)
					{
						khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
					}

					db.Add(khachHang);
					db.SaveChanges();

					// Gửi mail xác thực
					var message = $@"Chào mừng {khachHang.HoTen} đã đăng ký thành viên!<br/>
									Mã xác nhận 6 số của bạn là: <strong>{verificationCode}</strong><br/>
									Vui lòng nhập mã này trên trang xác nhận để kích hoạt tài khoản.";
					await _mailService.SendMail(khachHang.Email, "Mã xác nhận tài khoản", message);

					return RedirectToAction("XacNhanEmail", "KhachHang", new { email = khachHang.Email });
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("loi", "Lỗi khi đăng ký: " + ex.Message);
				}
			}
			return View();
		}
		#endregion

		[HttpGet]
		public IActionResult XacNhanEmail(string email)
		{
			ViewBag.Email = email;
			return View();
		}

		[HttpPost]
		public IActionResult XacNhanEmail(string email, string code)
		{
			var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.Email == email && kh.RandomKey == code);
			if (khachHang == null)
			{
				ModelState.AddModelError("loi", "Mã xác nhận không đúng hoặc email không tồn tại.");
				ViewBag.Email = email;
				return View();
			}

			khachHang.HieuLuc = true;
			db.Update(khachHang);
			db.SaveChanges();

			TempData["Message"] = "Xác thực email thành công! Bạn có thể đăng nhập ngay bây giờ.";
			return RedirectToAction("DangNhap");
		}

		#region Login
		[HttpGet]
		public IActionResult DangNhap(string? ReturnUrl)
		{
			ViewBag.ReturnUrl = ReturnUrl;
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> DangNhap(LoginVM model, string? ReturnUrl)
		{
			ViewBag.ReturnUrl = ReturnUrl;
			if (ModelState.IsValid)
			{
				var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);
				if (khachHang == null)
				{
					ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
				}
				else
				{
					if (!khachHang.HieuLuc)
					{
						ModelState.AddModelError("loi", "Tài khoản chưa được xác thực. Vui lòng kiểm tra email để kích hoạt tài khoản.");
					}
					else
					{
						if (khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
						{
							ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
						}
						else
						{
							var claims = new List<Claim> {
								new Claim(ClaimTypes.Email, khachHang.Email),
								new Claim(ClaimTypes.Name, khachHang.HoTen),
								new Claim(MySettingHelper.CLAIM_CUSTOMERID, khachHang.MaKh),
								new Claim(ClaimTypes.Role, khachHang.VaiTro == 1 ? "Admin" : "Customer")
							};

							var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
							var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

							await HttpContext.SignInAsync(claimsPrincipal);

							if (Url.IsLocalUrl(ReturnUrl))
							{
								return Redirect(ReturnUrl);
							}
							else
							{
								return Redirect("/");
							}
						}
					}
				}
			}
			return View();
		}
		#endregion

		#region Newsletter
		[HttpPost]
		public async Task<IActionResult> SubscribeNewsletter(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				return Json(new { success = false, message = "Vui lòng nhập email" });
			}

			var subject = "Chào mừng bạn đến với DIIXShop!";
			var content = $@"<h3>Cảm ơn bạn đã đăng ký nhận tin từ DIIXShop!</h3>
                             <p>Chúng tôi sẽ gửi cho bạn những thông tin khuyến mãi và sản phẩm mới nhất.</p>
                             <p>Chúc bạn một ngày tốt lành!</p>";

			var result = await _mailService.SendMail(email, subject, content);

			if (result == 1)
			{
				return Json(new { success = true, message = "Đăng ký nhận tin thành công! Vui lòng kiểm tra email của bạn." });
			}

			return Json(new { success = false, message = "Có lỗi xảy ra, vui lòng thử lại sau." });
		}
		#endregion

		[Microsoft.AspNetCore.Authorization.Authorize]
		public IActionResult Index()
		{
			var customerId = HttpContext.User.Claims.FirstOrDefault(p => p.Type == MySettingHelper.CLAIM_CUSTOMERID)?.Value;
			if (customerId == null) return RedirectToAction("DangNhap");
			
			var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
			if (khachHang == null) return RedirectToAction("DangNhap");

			var model = new CapNhatThongTinVM {
				HoTen = khachHang.HoTen,
				DiaChi = khachHang.DiaChi,
				DienThoai = khachHang.DienThoai,
				HinhMacDinh = khachHang.Hinh
			};

			return View(model);
		}

		[HttpPost]
		[Microsoft.AspNetCore.Authorization.Authorize]
		public IActionResult CapNhatThongTin(CapNhatThongTinVM model)
		{
			if (ModelState.IsValid)
			{
				var customerId = HttpContext.User.Claims.FirstOrDefault(p => p.Type == MySettingHelper.CLAIM_CUSTOMERID)?.Value;
				if (customerId == null) return RedirectToAction("DangNhap");
				
				var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
				if (khachHang == null) return RedirectToAction("DangNhap");

				khachHang.HoTen = model.HoTen;
				khachHang.DiaChi = model.DiaChi;
				khachHang.DienThoai = model.DienThoai;
				
				if (model.Hinh != null)
				{
					khachHang.Hinh = MyUtil.UploadHinh(model.Hinh, "KhachHang");
				}

				db.Update(khachHang);
				db.SaveChanges();
				
				TempData["Message"] = "Cập nhật thông tin thành công!";
				return RedirectToAction("Index");
			}
			return View("Index", model);
		}

		public async Task<IActionResult> DangXuat()
		{
			await HttpContext.SignOutAsync();
            HttpContext.Session.Remove(MySettingHelper.CART_KEY);
			return Redirect("/");
		}
	}
}

