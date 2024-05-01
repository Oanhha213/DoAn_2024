using DoAnNet7_2.Models;
using DoAnNet7_2.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DoAnNet7_2.Services;

namespace DoAnNet7_2.Controllers
{
    [Route("NhaTuyenDung")]
    public class NhaTuyenDungController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();

        [Authentication]
        [Route("")]
        [Route("NhaTuyenDung")]
        public IActionResult NhaTuyenDung()
        {
            var idTK = HttpContext.Session.GetInt32("IdTk");
            ViewBag.Hoten = db.Taikhoans.FirstOrDefault(x => x.IdTk == idTK)?.Hoten;
            return View();
        }

        [Route("TaoCongTy")]
        [HttpGet]
        public IActionResult TaoCongTy() 
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");

            if (!ID_TK.HasValue)
            {
                // Xử lý trường hợp người dùng không hợp lệ, có thể chuyển hướng hoặc xử lý lỗi khác
                return RedirectToAction("TrangLoi", "Home");
            }

            ViewBag.IdNn = new SelectList(db.Nganhnghes.ToList(), "IdNn", "Tennganhnghe");
            ViewBag.IdTt = new SelectList(db.Tinhthanhs.ToList(), "IdTt", "Tentt");

            var ct = new Congty
            {
                IdTk = ID_TK.Value
            };

            return View(ct);
        }

        [Route("TaoCongTy")]
        [HttpPost]
        public IActionResult TaoCongTy(Congty ct)
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");

            if (!ID_TK.HasValue)
            {
                // Xử lý trường hợp người dùng không hợp lệ, có thể chuyển hướng hoặc xử lý lỗi khác
                return RedirectToAction("Trangloi", "Home");
            }

            if (ct.UpLogo != null)
            {
                ct.Logo = LuuAnh.LuuAnhDaiDien(ct.UpLogo);
            }

            // Mục 3: Kiểm tra ModelState Errors
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            if (ModelState.IsValid)
            {
                ct.IdTk = ID_TK.Value;
                ct.Logo = LuuAnh.LuuAnhDaiDien(ct.UpLogo);
                db.Congties.Add(ct);
                db.SaveChanges();
                return RedirectToAction("NhaTuyenDung", "NhaTuyenDung");
            }
            return View(ct);
        }
    }
}
