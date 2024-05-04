using DoAnNet7_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DoAnNet7_2.Controllers
{
    public class HomeController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var DSBTDHien = db.Baituyendungs.Where(x => x.IdTtht == 2)
                            .Include(x => x.IdTthtNavigation)
                            .Include(x => x.IdLcvNavigation)
                            .Include(x => x.IdTglvNavigation)
                            .ToList();
            ViewBag.DSBTD = DSBTDHien;
            Debug.WriteLine("BTD:");

            foreach (var item in DSBTDHien)
            {
                Debug.WriteLine("ID: " + item.IdBtd); // Ví dụ về việc truy cập thuộc tính Id
                Debug.WriteLine("Name: " + item.Tencongviec); // In ra thông tin của mỗi đối tượng trong danh sách
            }
            return View();
        }

        public IActionResult Trangloi()
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