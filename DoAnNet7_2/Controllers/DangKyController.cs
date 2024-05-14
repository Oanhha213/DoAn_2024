using DoAnNet7_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DoAnNet7_2.Services;

namespace DoAnNet7_2.Controllers
{
    public class DangKyController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();

        [Route("DangKy")]
        [HttpGet]
        public IActionResult DangKy()
        {
            // Tạo SelectList cho loại tài khoản
            var loaiTK = new[]
            {
                new { IdLtk = 2, Text = "Nhà tuyển dụng" },
                new { IdLtk = 3, Text = "Người tìm việc" }
            };
            ViewBag.IdLtk = new SelectList(loaiTK, "IdLtk", "Text");
            return View();
        }

        [Route("DangKy")]
        [HttpPost]
        public IActionResult DangKy(Taikhoan tk)
        {
            if (ModelState.IsValid)
            {
                db.Taikhoans.Add(tk);
                db.SaveChanges();
                var Id_TK = tk.IdTk;
                HttpContext.Session.SetInt32("IdTk", Id_TK);

                // Chuyển hướng đến action UpAnhDaiDien và truyền vai trò của người dùng
                return RedirectToAction("UpAnhDaiDien", "DangKy");
            }
            return View(tk);
        }

        [Route("UpAnhDaiDien")]
        [HttpGet]
        public IActionResult UpAnhDaiDien()
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");

            if (!ID_TK.HasValue)
            {
                // Xử lý trường hợp người dùng không hợp lệ, có thể chuyển hướng hoặc xử lý lỗi khác
                //return View();
                return RedirectToAction("Trangloi", "Home");
            }

            var anhDD = new Anhdaidien
            {
                IdTk = ID_TK.Value
            };

            return View(anhDD);
        }

        [Route("UpAnhDaiDien")]
        [HttpPost]

        public IActionResult UpAnhDaiDien(Anhdaidien anhdd)
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");

            if (!ID_TK.HasValue)
            {
                // Xử lý trường hợp người dùng không hợp lệ, có thể chuyển hướng hoặc xử lý lỗi khác
                return RedirectToAction("Trangloi", "Home");
            }

            if (anhdd.UpAnhDaiDien != null)
            {
                anhdd.Tenanh = LuuAnh.LuuAnhDaiDien(anhdd.UpAnhDaiDien);
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
                anhdd.IdTk = ID_TK.Value;
                anhdd.Tenanh = LuuAnh.LuuAnhDaiDien(anhdd.UpAnhDaiDien);
                db.Anhdaidiens.Add(anhdd);
                db.SaveChanges();

                // Kiểm tra loại tài khoản và chuyển hướng tới action tương ứng
                var tk = db.Taikhoans.FirstOrDefault(x => x.IdTk == ID_TK);
                // Dựa vào vai trò của người dùng để chuyển hướng tới action tương ứng
                if (tk != null)
                {
                    if (tk.IdLtk == 2)
                    {
                        return RedirectToAction("TaoCongTy", "NhaTuyenDung");
                    }
                    else if (tk.IdLtk == 3)
                    {
                        return RedirectToAction("ThemCV", "NguoiTimViec");
                    }
                }
            }
            return View(anhdd);
        }
    }
}
