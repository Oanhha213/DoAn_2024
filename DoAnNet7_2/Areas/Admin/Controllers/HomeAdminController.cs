using DoAnNet7_2.Models;
using DoAnNet7_2.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace DoAnNet7_2.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    public class HomeAdminController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();

        //[Authentication]
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            var countNTD = db.Taikhoans.AsNoTracking().Where(x => x.IdLtk == 2).Distinct().Count();
            ViewBag.countNTD = countNTD;
            var countNTV = db.Taikhoans.AsNoTracking().Where(x => x.IdLtk == 3).Distinct().Count();
            ViewBag.countNTV = countNTV;
            var countBTD = db.Baituyendungs.AsNoTracking().Select(x => x.IdBtd).Distinct().Count();
            ViewBag.countBTD = countBTD;

            var BTDMoi = db.Baituyendungs.OrderByDescending(b => b.CreateAt)
                        .Include(x => x.IdNnNavigation)
                        .Include(x => x.IdLcvNavigation)
                        .Include(x => x.IdTkNavigation)
                        .Take(5) // Chỉ lấy 5 bài đăng mới nhất
                        .ToList();
            ViewBag.BTDMoi = BTDMoi;
            return View();
        }

    }
}
