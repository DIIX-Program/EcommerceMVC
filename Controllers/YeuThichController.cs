using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using EcommerceMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMVC.Controllers
{
    [Authorize]
    public class YeuThichController : Controller
    {
        private readonly Hshop2023Context db;

        public YeuThichController(Hshop2023Context context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySettingHelper.CLAIM_CUSTOMERID).Value;
            var list = db.YeuThiches
                .Include(yt => yt.MaHhNavigation)
                .Where(yt => yt.MaKh == customerId)
                .ToList();
            return View(list);
        }

        public IActionResult Add(int id)
        {
            var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySettingHelper.CLAIM_CUSTOMERID).Value;
            
            var exists = db.YeuThiches.Any(yt => yt.MaHh == id && yt.MaKh == customerId);
            if (!exists)
            {
                var yt = new YeuThich
                {
                    MaHh = id,
                    MaKh = customerId,
                    NgayChon = DateTime.Now
                };
                db.Add(yt);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySettingHelper.CLAIM_CUSTOMERID).Value;
            var yt = db.YeuThiches.SingleOrDefault(p => p.MaYt == id && p.MaKh == customerId);
            if (yt != null)
            {
                db.Remove(yt);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
