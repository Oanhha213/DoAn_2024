using DoAnNet7_2.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoAnNet7_2.Areas.Admin.ViewComponents
{
    public class AdminInfoViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int idTk;
            if (HttpContext.Session.GetInt32("IdTk").HasValue)
            {
                idTk = HttpContext.Session.GetInt32("IdTk").Value;
                var hoten = HttpContext.Session.GetString("Hoten");
                var tenanh = HttpContext.Session.GetString("Tenanh");
                var model = new Models.ViewModels.TaiKhoanAnhViewModel
                {
                    IdTk = idTk,
                    Hoten = hoten,
                    Tenanh = tenanh
                };
                return View("AdminInfo", model);
            }
            return View();
        }
    }
}
