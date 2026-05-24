using Microsoft.AspNetCore.Mvc;

namespace EcommerceMVC.ViewComponents
{
    public class PopupSaleViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
