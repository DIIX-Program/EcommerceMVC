using EcommerceMVC.Data;
using EcommerceMVC.Helpers;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace EcommerceMVC.ViewComponents
{
	public class CartViewComponent : ViewComponent
	{
		private readonly Hshop2023Context db;

		public CartViewComponent(Hshop2023Context context)
		{
			db = context;
		}

		public IViewComponentResult Invoke()
		{
			var customerId = HttpContext.User.Claims.FirstOrDefault(p => p.Type == MySettingHelper.CLAIM_CUSTOMERID)?.Value;
			
			int quantity = 0;
			double total = 0.0;

			if (!string.IsNullOrEmpty(customerId))
			{
				var cart = db.GioHangs
					.Where(g => g.MaKh == customerId)
					.Select(g => new
					{
						g.SoLuong,
						DonGia = g.MaHhNavigation.DonGia ?? 0
					})
					.ToList();

				quantity = cart.Sum(c => c.SoLuong);
				total = cart.Sum(c => c.SoLuong * c.DonGia);
			}

			return View(new CartModel
			{
				Quantity = quantity,
				Total = total
			});
		}
	}
}
