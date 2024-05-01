using DoAnNet7_2.Models;
using Microsoft.AspNetCore.Mvc;
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