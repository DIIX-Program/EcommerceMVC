using EcommerceMVC.Data;
using EcommerceMVC.Models;
using EcommerceMVC.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("diix/[action]")]
    public class DiixController : Controller
    {
        private readonly Hshop2023Context _db;

        public DiixController(Hshop2023Context db)
        {
            _db = db;
        }

        [Route("/diix")]
        public IActionResult Index()
        {
            return View();
        }

        #region Hàng Hóa
        public IActionResult HangHoa(string searchKey, int page = 1)
        {
            int pageSize = 10;
            var query = _db.HangHoas.Include(h => h.MaLoaiNavigation).AsQueryable();

            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(h => h.TenHh.Contains(searchKey) || h.MaLoaiNavigation.TenLoai.Contains(searchKey));
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var data = query
                .OrderByDescending(h => h.IsBestseller)
                .ThenBy(h => h.SortOrder)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.SearchKey = searchKey;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(data);
        }

        [HttpPost]
        public IActionResult UpdateBestsellerStatus(int id, bool status)
        {
            var item = _db.HangHoas.Find(id);
            if (item != null)
            {
                item.IsBestseller = status;
                _db.SaveChanges();
                return Json(new { success = true, message = $"Đã cập nhật Bestseller sản phẩm #{id}" });
            }
            return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
        }

        [HttpPost]
        public IActionResult UpdateSortOrder(int id, int sortOrder)
        {
            var item = _db.HangHoas.Find(id);
            if (item != null)
            {
                item.SortOrder = sortOrder;
                _db.SaveChanges();
            }
            return RedirectToAction("HangHoa");
        }

        [HttpPost]
        public IActionResult DeleteHangHoa(int id)
        {
            var item = _db.HangHoas.Find(id);
            if (item != null)
            {
                _db.HangHoas.Remove(item);
                _db.SaveChanges();
            }
            return RedirectToAction("HangHoa");
        }

        public IActionResult CreateHangHoa()
        {
            ViewBag.Loais = _db.Loais.ToList();
            ViewBag.NhaCungCaps = _db.NhaCungCaps.ToList();
            return View(new HangHoa { NgaySx = DateTime.Now });
        }

        [HttpPost]
        public async Task<IActionResult> CreateHangHoa(HangHoa model, IFormFile? HinhUpload)
        {
            ModelState.Remove("MaLoaiNavigation");
            ModelState.Remove("MaNccNavigation");
            if (ModelState.IsValid)
            {
                if (HinhUpload != null && HinhUpload.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(HinhUpload.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "Hinh", "HangHoa", fileName);
                    
                    var dir = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhUpload.CopyToAsync(stream);
                    }
                    model.Hinh = fileName;
                }
                else
                {
                    model.Hinh = "default.png";
                }

                _db.HangHoas.Add(model);
                await _db.SaveChangesAsync();
                return RedirectToAction("HangHoa");
            }

            ViewBag.Loais = _db.Loais.ToList();
            ViewBag.NhaCungCaps = _db.NhaCungCaps.ToList();
            return View(model);
        }

        public IActionResult EditHangHoa(int id)
        {
            var item = _db.HangHoas.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            ViewBag.Loais = _db.Loais.ToList();
            ViewBag.NhaCungCaps = _db.NhaCungCaps.ToList();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> EditHangHoa(HangHoa model, IFormFile? HinhUpload)
        {
            ModelState.Remove("MaLoaiNavigation");
            ModelState.Remove("MaNccNavigation");
            if (ModelState.IsValid)
            {
                var existing = _db.HangHoas.Find(model.MaHh);
                if (existing == null)
                {
                    return NotFound();
                }

                existing.TenHh = model.TenHh;
                existing.TenAlias = model.TenAlias;
                existing.MaLoai = model.MaLoai;
                existing.MoTaDonVi = model.MoTaDonVi;
                existing.DonGia = model.DonGia;
                existing.NgaySx = model.NgaySx;
                existing.GiamGia = model.GiamGia;
                existing.IsBestseller = model.IsBestseller;
                existing.SortOrder = model.SortOrder;
                existing.MoTa = model.MoTa;
                existing.MaNcc = model.MaNcc;

                if (HinhUpload != null && HinhUpload.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(HinhUpload.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "Hinh", "HangHoa", fileName);

                    var dir = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhUpload.CopyToAsync(stream);
                    }
                    existing.Hinh = fileName;
                }

                await _db.SaveChangesAsync();
                return RedirectToAction("HangHoa");
            }

            ViewBag.Loais = _db.Loais.ToList();
            ViewBag.NhaCungCaps = _db.NhaCungCaps.ToList();
            return View(model);
        }
        #endregion

        #region Loại
        public IActionResult Loai(string searchKey, int page = 1, int? editId = null)
        {
            int pageSize = 10;
            var query = _db.Loais.AsQueryable();

            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(l => l.TenLoai.Contains(searchKey));
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var skipCount = (page - 1) * pageSize;
            if (skipCount < 0) skipCount = 0;

            var data = query
                .OrderBy(l => l.MaLoai)
                .Skip(skipCount)
                .Take(pageSize)
                .ToList();

            ViewBag.SearchKey = searchKey;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            if (editId.HasValue)
            {
                ViewBag.EditItem = _db.Loais.Find(editId.Value);
            }

            return View(data);
        }

        [HttpPost]
        public IActionResult CreateLoai(Loai model)
        {
            if (ModelState.IsValid)
            {
                _db.Loais.Add(model);
                _db.SaveChanges();
            }
            return RedirectToAction("Loai");
        }

        [HttpPost]
        public IActionResult EditLoai(Loai model)
        {
            if (ModelState.IsValid)
            {
                var existing = _db.Loais.Find(model.MaLoai);
                if (existing != null)
                {
                    existing.TenLoai = model.TenLoai;
                    existing.TenLoaiAlias = model.TenLoaiAlias;
                    existing.MoTa = model.MoTa;
                    _db.SaveChanges();
                }
            }
            return RedirectToAction("Loai");
        }

        [HttpPost]
        public IActionResult DeleteLoai(int id)
        {
            var item = _db.Loais.Find(id);
            if (item != null)
            {
                _db.Loais.Remove(item);
                _db.SaveChanges();
            }
            return RedirectToAction("Loai");
        }
        #endregion

        #region Nhân Viên
        public IActionResult NhanVien(string searchKey, int page = 1, string? editId = null)
        {
            int pageSize = 10;
            var query = _db.NhanViens.AsQueryable();

            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(nv => nv.HoTen.Contains(searchKey) || nv.MaNv.Contains(searchKey) || nv.Email.Contains(searchKey));
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var skipCount = (page - 1) * pageSize;
            if (skipCount < 0) skipCount = 0;

            var data = query
                .OrderBy(nv => nv.MaNv)
                .Skip(skipCount)
                .Take(pageSize)
                .ToList();

            ViewBag.SearchKey = searchKey;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            if (!string.IsNullOrEmpty(editId))
            {
                ViewBag.EditItem = _db.NhanViens.Find(editId);
            }

            return View(data);
        }

        [HttpPost]
        public IActionResult CreateNhanVien(NhanVien model)
        {
            if (ModelState.IsValid)
            {
                _db.NhanViens.Add(model);
                _db.SaveChanges();
            }
            return RedirectToAction("NhanVien");
        }

        [HttpPost]
        public IActionResult EditNhanVien(NhanVien model)
        {
            if (ModelState.IsValid)
            {
                var existing = _db.NhanViens.Find(model.MaNv);
                if (existing != null)
                {
                    existing.HoTen = model.HoTen;
                    existing.Email = model.Email;
                    if (!string.IsNullOrEmpty(model.MatKhau))
                    {
                        existing.MatKhau = model.MatKhau;
                    }
                    _db.SaveChanges();
                }
            }
            return RedirectToAction("NhanVien");
        }

        [HttpPost]
        public IActionResult DeleteNhanVien(string id)
        {
            var item = _db.NhanViens.Find(id);
            if (item != null)
            {
                _db.NhanViens.Remove(item);
                _db.SaveChanges();
            }
            return RedirectToAction("NhanVien");
        }
        #endregion

        #region Khách Hàng
        public IActionResult KhachHang(string searchKey, int page = 1)
        {
            int pageSize = 10;
            var query = _db.KhachHangs.AsQueryable();

            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(kh => kh.HoTen.Contains(searchKey) || kh.MaKh.Contains(searchKey) || kh.Email.Contains(searchKey) || kh.DienThoai.Contains(searchKey));
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var skipCount = (page - 1) * pageSize;
            if (skipCount < 0) skipCount = 0;

            var data = query
                .OrderBy(kh => kh.MaKh)
                .Skip(skipCount)
                .Take(pageSize)
                .ToList();

            ViewBag.SearchKey = searchKey;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(data);
        }

        [HttpPost]
        public IActionResult CreateKhachHang(KhachHang model)
        {
            if (ModelState.IsValid)
            {
                // Ensure key fields are set
                model.NgaySinh = model.NgaySinh == default ? DateTime.Now : model.NgaySinh;
                model.HieuLuc = true;
                model.VaiTro = 0; // Customer role
                
                var randomKey = MyUtil.GenerateRandomKey();
                model.RandomKey = randomKey;
                if (!string.IsNullOrEmpty(model.MatKhau))
                {
                    model.MatKhau = model.MatKhau.ToMd5Hash(randomKey);
                }
                else
                {
                    model.MatKhau = "123456".ToMd5Hash(randomKey);
                }

                _db.KhachHangs.Add(model);
                _db.SaveChanges();
            }
            return RedirectToAction("KhachHang");
        }

        [HttpPost]
        public IActionResult DeleteKhachHang(string id)
        {
            var item = _db.KhachHangs.Find(id);
            if (item != null)
            {
                _db.KhachHangs.Remove(item);
                _db.SaveChanges();
            }
            return RedirectToAction("KhachHang");
        }
        #endregion

        #region Đơn Hàng
        public IActionResult HoaDon(int? status, string searchKey, int page = 1)
        {
            int pageSize = 10;
            var query = _db.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.MaTrangThaiNavigation)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(h => h.MaTrangThai == status.Value);
            }

            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(h => 
                    h.HoTen.Contains(searchKey) || 
                    h.DiaChi.Contains(searchKey) || 
                    h.MaKh.Contains(searchKey) ||
                    h.MaHd.ToString().Contains(searchKey) ||
                    (h.MaKhNavigation != null && h.MaKhNavigation.HoTen.Contains(searchKey))
                );
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var skipCount = (page - 1) * pageSize;
            if (skipCount < 0) skipCount = 0;

            var data = query
                .OrderByDescending(h => h.NgayDat)
                .Skip(skipCount)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentStatus = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchKey = searchKey;
            ViewBag.TrangThais = _db.TrangThais.ToList();

            return View(data);
        }

        public IActionResult ChiTietHoaDon(int id)
        {
            var order = _db.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.MaTrangThaiNavigation)
                .SingleOrDefault(h => h.MaHd == id);

            if (order == null)
            {
                return NotFound();
            }

            var details = _db.ChiTietHds
                .Include(d => d.MaHhNavigation)
                .Where(d => d.MaHd == id)
                .ToList();

            ViewBag.Order = order;
            ViewBag.TrangThais = _db.TrangThais.ToList();

            return View(details);
        }

        [HttpPost]
        public IActionResult UpdateTrangThaiHoaDon(int id, int maTrangThai)
        {
            var order = _db.HoaDons.Find(id);
            if (order != null)
            {
                order.MaTrangThai = maTrangThai;
                if (maTrangThai == 3) // Assuming status 3 is "Delivered"
                {
                    order.NgayGiao = DateTime.Now;
                }
                _db.SaveChanges();
            }
            return RedirectToAction("ChiTietHoaDon", new { id = id });
        }
        #endregion
    }
}
