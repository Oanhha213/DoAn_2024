using DoAnNet7_2.Models;
using DoAnNet7_2.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            //var idTk = HttpContext.Session.GetInt32("IdTk");
            //ViewBag.IdTk = idTk;
            //var hoten = HttpContext.Session.GetString("Hoten");
            //ViewBag.Hoten = hoten;
            //var tenanh = HttpContext.Session.GetString("Tenanh");
            //ViewBag.Tenanh = tenanh;
            //Debug.WriteLine("ID:" + idTk);
            //Debug.WriteLine("Hoten:" + hoten);
            //Debug.WriteLine("Tenanh:" + tenanh);
            return View();
        }

    }
}
