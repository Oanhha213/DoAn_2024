using DoAnNet7_2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DoAnNet7_2.Controllers
{
    public class AccessController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();

        [Route("DangNhap")]
        [HttpGet]

        public IActionResult DangNhap()
        {
            if (HttpContext.Session.GetString("Email") == null)
            {
                return View();
            }
            else
                return RedirectToAction("Index", "Home");
        }

        [Route("DangNhap")]
        [HttpPost]
        public IActionResult DangNhap(Taikhoan taikhoan)
        {

            var obj = db.Taikhoans.Where(x => x.Email == taikhoan.Email && x.Matkhau == taikhoan.Matkhau).FirstOrDefault();
            if (obj != null)
            {
                var anhdd = db.Anhdaidiens.FirstOrDefault(x => x.IdTk == obj.IdTk)?.Tenanh;
                if (obj.IdLtk == 1)
                {
                    HttpContext.Session.SetString("Email", obj.Email.ToString());
                    HttpContext.Session.SetString("Hoten", obj.Hoten.ToString());
                    HttpContext.Session.SetInt32("IdTk", obj.IdTk);
                    if (anhdd != null)
                    {
                        HttpContext.Session.SetString("Tenanh", anhdd.ToString());
                    }

                    return RedirectToAction("index", "admin");
                }
                var anh = db.Anhdaidiens.Where(x => x.IdTk == obj.IdTk).FirstOrDefault();
                if (obj.IdLtk == 2)
                {
                    HttpContext.Session.SetString("Email", obj.Email.ToString());
                    HttpContext.Session.SetString("Hoten", obj.Hoten.ToString());
                    HttpContext.Session.SetInt32("IdTk", obj.IdTk);
                    if (anh != null)
                    {
                        HttpContext.Session.SetInt32("IdAnhdd", anh.IdAnhdd);
                        HttpContext.Session.SetString("Tenanh", anh.Tenanh.ToString());
                    }
                    return RedirectToAction("NhaTuyenDung", "NhaTuyenDung");
                }
                else
                {
                    HttpContext.Session.SetString("Email", obj.Email.ToString());
                    HttpContext.Session.SetString("Hoten", obj.Hoten.ToString());
                    HttpContext.Session.SetInt32("IdTk", obj.IdTk);
                    if (anh != null)
                    {
                        HttpContext.Session.SetInt32("IdAnhdd", anh.IdAnhdd);
                        HttpContext.Session.SetString("Tenanh", anh.Tenanh.ToString());
                    }
                    return RedirectToAction("NguoiTimViec", "NguoiTimViec");
                }
            }
            return View();
        }

        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("Email");
            HttpContext.Session.Remove("Hoten");
            HttpContext.Session.Remove("IdTk");
            HttpContext.Session.Remove("IdAnhdd");
            HttpContext.Session.Remove("Tenanh");
            return RedirectToAction("Index", "Home");
        }
    }
}
