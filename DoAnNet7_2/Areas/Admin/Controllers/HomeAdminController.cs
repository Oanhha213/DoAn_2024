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
    [Route("admin/homeadmin")]
    public class HomeAdminController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();

        //[Authentication]
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            //var countNTD = db.Taikhoans.AsNoTracking().Where(x => x.IdLtk == 2).Distinct().Count();
            //ViewBag.countNTD = countNTD;
            // Lấy số lượng tài khoản mới trong tháng trước
            var countNTDLastMonth = db.Taikhoans
                .Where(x => x.IdLtk == 2 && x.CreateAt >= DateTime.Today.AddMonths(-1) && x.CreateAt < DateTime.Today)
                .Distinct()
                .Count();

            // Lấy số lượng tài khoản mới trong tháng trước đó
            var countNTDSecondLastMonth = db.Taikhoans
                .Where(x => x.IdLtk == 2 && x.CreateAt >= DateTime.Today.AddMonths(-2) && x.CreateAt < DateTime.Today.AddMonths(-1))
                .Distinct()
                .Count();

            // Tính toán phần trăm tăng trưởng hoặc giảm
            double percentChangeNTD = 0.0;
            if (countNTDSecondLastMonth != 0)
            {
                percentChangeNTD = ((double)(countNTDLastMonth - countNTDSecondLastMonth) / countNTDSecondLastMonth) * 100;
            }

            ViewBag.countNTDLastMonth = countNTDLastMonth;
            ViewBag.countNTDSecondLastMonth = countNTDSecondLastMonth;
            ViewBag.percentChangeNTD = percentChangeNTD;

            //var countNTV = db.Taikhoans.AsNoTracking().Where(x => x.IdLtk == 3).Distinct().Count();
            //ViewBag.countNTV = countNTV;

            var countNTVLastMonth = db.Taikhoans
                .Where(x => x.IdLtk == 3 && x.CreateAt >= DateTime.Today.AddMonths(-1) && x.CreateAt < DateTime.Today)
                .Distinct()
                .Count();

            // Lấy số lượng tài khoản mới trong tháng trước đó
            var countNTVSecondLastMonth = db.Taikhoans
                .Where(x => x.IdLtk == 3 && x.CreateAt >= DateTime.Today.AddMonths(-2) && x.CreateAt < DateTime.Today.AddMonths(-1))
                .Distinct()
                .Count();

            // Tính toán phần trăm tăng trưởng hoặc giảm
            double percentChangeNTV = 0.0;
            if (countNTDSecondLastMonth != 0)
            {
                percentChangeNTV = ((double)(countNTVLastMonth - countNTVSecondLastMonth) / countNTVSecondLastMonth) * 100;
            }

            ViewBag.countNTVLastMonth = countNTVLastMonth;
            ViewBag.countNTVSecondLastMonth = countNTVSecondLastMonth;
            ViewBag.percentChangeNTV = percentChangeNTV;

            //var countBTD = db.Baituyendungs.AsNoTracking().Select(x => x.IdBtd).Distinct().Count();
            //ViewBag.countBTD = countBTD;

            var countBTDLastMonth = db.Baituyendungs
                .Where(x => x.CreateAt >= DateTime.Today.AddMonths(-1) && x.CreateAt < DateTime.Today)
                .GroupBy(x => x.CreateAt)
                .Select(group => group.FirstOrDefault())
                .Count();

            var countBTDSecondLastMonth = db.Baituyendungs
                .Where(x => x.CreateAt >= DateTime.Today.AddMonths(-2) && x.CreateAt < DateTime.Today.AddMonths(-1))
                .GroupBy(x => x.CreateAt)
                .Select(group => group.FirstOrDefault())
                .Count();

            // Tính toán phần trăm tăng trưởng hoặc giảm
            double percentChangeBTD = 0.0;
            if (countBTDSecondLastMonth != 0)
            {
                percentChangeBTD = ((double)(countBTDLastMonth - countBTDSecondLastMonth) / countBTDSecondLastMonth) * 100;
            }
            Debug.WriteLine("Tháng 3: " + countBTDSecondLastMonth);
            Debug.WriteLine("Tháng 4: " + countBTDLastMonth);
            Debug.WriteLine("Phần trăm: " + percentChangeBTD);

            ViewBag.countBTDLastMonth = countBTDLastMonth;
            ViewBag.countBTDSecondLastMonth = countBTDSecondLastMonth;
            ViewBag.percentChangeBTD = percentChangeBTD;

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
