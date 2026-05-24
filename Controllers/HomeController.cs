using System.Diagnostics;
using EcommerceMVC.Models;
using EcommerceMVC.Data;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Hshop2023Context _db;

        public HomeController(ILogger<HomeController> logger, Hshop2023Context db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            var bestSellers = _db.HangHoas
                .Where(h => h.IsBestseller)
                .OrderBy(h => h.SortOrder)
                .ToList();
            return View(bestSellers);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
