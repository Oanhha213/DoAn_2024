using DoAnNet7_2.Models;
using DoAnNet7_2.Models.Authentication;
using DoAnNet7_2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace DoAnNet7_2.Areas.Admin.Controllers
{
    [Area("admin")]
    public class AnhController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();

        [Authentication]
        [Route("ThemAnhDaiDien")]
        [HttpGet]
        public IActionResult ThemAnhDaiDien()
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");
            Debug.WriteLine("IDTK: " + ID_TK);
            var idLtk = db.Taikhoans.FirstOrDefault(x => x.IdTk == ID_TK)?.IdLtk;
            ViewBag.IdLtk = idLtk;
            if (ViewBag.IdLtk != null)
            {
                Debug.WriteLine("IdLtk: " + idLtk);
            }
            return View();
        }

        [Authentication]
        [Route("ThemAnhDaiDien")]
        [HttpPost]

        public IActionResult ThemAnhDaiDien(Anhdaidien anhdd)
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");

            if (!ID_TK.HasValue)
            {
                // Xử lý trường hợp người dùng không hợp lệ, có thể chuyển hướng hoặc xử lý lỗi khác
                return RedirectToAction("Trangloi", "Home");
            }

            // Mục 3: Kiểm tra ModelState Errors
            if (anhdd.UpAnhDaiDien != null && ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine(error.ErrorMessage);
                }
            }
                var IdLTK = db.Taikhoans.FirstOrDefault(x => x.IdTk == ID_TK)?.IdLtk;
                ViewBag.IdLtk = IdLTK;

            if (ModelState.IsValid)
            {
                anhdd.IdTk = ID_TK.Value;
                anhdd.Tenanh = LuuAnh.LuuAnhDaiDien(anhdd.UpAnhDaiDien);
                db.Anhdaidiens.Add(anhdd);
                db.SaveChanges();
                // Dựa vào vai trò của người dùng để chuyển hướng tới action tương ứng
                ViewBag.SuccessMessage = "Thêm tài khoản thành công.";
                ViewBag.ThemTKThanhCong = true;
            }
            return View(anhdd);
        }

        //                        if (IdLTK != null)
        //                {
        //                    if (IdLTK == 2)
        //                    {
        //                        return RedirectToAction("DSNhaTuyenDung", "TKNhaTuyenDung");
        //    }
        //                    else
        //                    {
        //                        return RedirectToAction("DSNguoiTimViec", "TKNguoiTimViec");
        //}

        //                }

        [Authentication]
        [Route("SuaAnhDD")]
        [HttpGet]
        public IActionResult SuaAnhDD(int idTK)
        {
            var anhDD = db.Anhdaidiens.FirstOrDefault(a => a.IdTk == idTK);
            var Id_TK = idTK;
            HttpContext.Session.SetInt32("IdTk", Id_TK);
            if (anhDD == null)
            {
                return RedirectToAction("ThemAnhDaiDien", "Anh");
            }

            return View(anhDD);
        }

        [Authentication]
        [Route("SuaAnhDD")]
        [HttpPost]
        public IActionResult SuaAnhDD(Anhdaidien anhDD)
        {
            var existingAnhDD = db.Anhdaidiens.FirstOrDefault(a => a.IdTk == anhDD.IdTk);
            if (existingAnhDD == null)
            {
                var ID_TK = HttpContext.Session.GetInt32("IdTk");
                return RedirectToAction("ThemAnhDaiDien", "Anh");
            }

            if (anhDD.UpAnhDaiDien != null && ModelState.IsValid)
            {
                ViewBag.IdLtk = db.Taikhoans.FirstOrDefault(x => x.IdTk == anhDD.IdTk)?.IdLtk;
                // Xóa ảnh cũ nếu cần

                // Lưu ảnh mới
                existingAnhDD.Tenanh = LuuAnh.LuuAnhDaiDien(anhDD.UpAnhDaiDien);

                // Cập nhật thông tin ảnh đại diện trong cơ sở dữ liệu
                db.SaveChanges();
                ViewBag.MessageType = "success";
                ViewBag.Message = "Sửa thành công";

                //return RedirectToAction("DanhSachTaiKhoan", "DanhSachTaiKhoan");
            }
            else
            {
                // Truyền thông điệp thất bại trực tiếp tới view
                ViewBag.MessageType = "error";
                ViewBag.Message = "Vui lòng kiểm tra lại thông tin";
                // Trả về lại view với dữ liệu cũ
                return View(existingAnhDD);
            }
            return View(existingAnhDD);
        }

    }
}
