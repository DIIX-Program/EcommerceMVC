using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using EcommerceMVC.Models;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcommerceMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly Hshop2023Context db;

        public CartController(Hshop2023Context context)
        {
            db = context;
        }

        public List<CartItem> Cart
        {
            get
            {
                var customerId = HttpContext.User.Claims.FirstOrDefault(p => p.Type == MySettingHelper.CLAIM_CUSTOMERID)?.Value;
                if (string.IsNullOrEmpty(customerId))
                {
                    return new List<CartItem>();
                }

                return db.GioHangs
                    .Where(g => g.MaKh == customerId)
                    .Select(g => new CartItem
                    {
                        MaHh = g.MaHh,
                        TenHh = g.MaHhNavigation.TenHh,
                        Hinh = g.MaHhNavigation.Hinh ?? "",
                        DonGia = g.MaHhNavigation.DonGia ?? 0,
                        SoLuong = g.SoLuong
                    })
                    .ToList();
            }
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Index()
        {
            return View(Cart);
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var customerId = HttpContext.User.Claims.FirstOrDefault(p => p.Type == MySettingHelper.CLAIM_CUSTOMERID)?.Value;
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }

            var gioHangItem = db.GioHangs.SingleOrDefault(g => g.MaKh == customerId && g.MaHh == id);
            if (gioHangItem == null)
            {
                var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hangHoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                    return Redirect("/404");
                }
                gioHangItem = new GioHang
                {
                    MaKh = customerId,
                    MaHh = id,
                    SoLuong = quantity
                };
                db.GioHangs.Add(gioHangItem);
            }
            else
            {
                gioHangItem.SoLuong += quantity;
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult RemoveCart(int id)
        {
            var customerId = HttpContext.User.Claims.FirstOrDefault(p => p.Type == MySettingHelper.CLAIM_CUSTOMERID)?.Value;
            if (!string.IsNullOrEmpty(customerId))
            {
                var item = db.GioHangs.SingleOrDefault(g => g.MaKh == customerId && g.MaHh == id);
                if (item != null)
                {
                    db.GioHangs.Remove(item);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var customerId = HttpContext.User.Claims.FirstOrDefault(p => p.Type == MySettingHelper.CLAIM_CUSTOMERID)?.Value;
            if (!string.IsNullOrEmpty(customerId))
            {
                var item = db.GioHangs.SingleOrDefault(g => g.MaKh == customerId && g.MaHh == id);
                if (item != null)
                {
                    item.SoLuong = quantity > 0 ? quantity : 1;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        public IActionResult UpdateQuantityAjax(int id, int quantity)
        {
            var customerId = HttpContext.User.Claims.FirstOrDefault(p => p.Type == MySettingHelper.CLAIM_CUSTOMERID)?.Value;
            if (string.IsNullOrEmpty(customerId))
            {
                return Json(new { success = false, message = "Not authenticated" });
            }

            var item = db.GioHangs.SingleOrDefault(g => g.MaKh == customerId && g.MaHh == id);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    db.GioHangs.Remove(item);
                }
                else
                {
                    item.SoLuong = quantity;
                }
                db.SaveChanges();
            }

            var gioHang = Cart;
            var currentItem = gioHang.SingleOrDefault(p => p.MaHh == id);
            return Json(new { 
                success = true, 
                total = gioHang.Sum(p => p.ThanhTien), 
                itemTotal = currentItem != null ? currentItem.ThanhTien : 0, 
                count = gioHang.Count 
            });
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if (Cart.Count == 0)
            {
                TempData["Message"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm trước khi thanh toán.";
                return RedirectToAction("Index");
            }

            var customerIdClaim = HttpContext.User.Claims.FirstOrDefault(p => p.Type == MySettingHelper.CLAIM_CUSTOMERID);
            if (customerIdClaim == null) 
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }
            var customerId = customerIdClaim.Value;
            var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
            ViewBag.KhachHang = khachHang;

            return View(Cart);
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var customerIdClaim = HttpContext.User.Claims.FirstOrDefault(p => p.Type == MySettingHelper.CLAIM_CUSTOMERID);
                if (customerIdClaim == null) 
                {
                    return RedirectToAction("DangNhap", "KhachHang");
                }
                var customerId = customerIdClaim.Value;
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
                
                var hoadon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = model.HoTen ?? khachHang.HoTen,
                    DiaChi = model.DiaChi ?? khachHang.DiaChi,
                    NgayDat = DateTime.Now,
                    CachThanhToan = Request.Form["CachThanhToan"].ToString() ?? "COD",
                    CachVanChuyen = "GHTK",
                    MaTrangThai = 0, // Mới đặt hàng
                    GhiChu = $"ĐT: {model.DienThoai ?? khachHang.DienThoai}. {model.GhiChu}"
                };
                
                if (model.GiongKhachHang)
                {
                    hoadon.HoTen = khachHang.HoTen;
                    hoadon.DiaChi = khachHang.DiaChi;
                }

                db.Database.BeginTransaction();
                try
                {
                    db.Add(hoadon);
                    db.SaveChanges();

                    var cthds = new List<ChiTietHd>();
                    foreach (var item in Cart)
                    {
                        cthds.Add(new ChiTietHd
                        {
                            MaHd = hoadon.MaHd,
                            MaHh = item.MaHh,
                            DonGia = item.DonGia,
                            SoLuong = item.SoLuong,
                            GiamGia = 0
                        });
                    }
                    db.AddRange(cthds);
                    db.SaveChanges();

                    // Clear database shopping cart
                    var cartItems = db.GioHangs.Where(g => g.MaKh == customerId);
                    db.GioHangs.RemoveRange(cartItems);
                    db.SaveChanges();

                    db.Database.CommitTransaction();

                    return View("Success");
                }
                catch
                {
                    db.Database.RollbackTransaction();
                }
            }

            return View(Cart);
        }
    }
}
