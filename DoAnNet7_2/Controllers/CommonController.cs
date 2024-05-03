using DoAnNet7_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DoAnNet7_2.Controllers
{
    [Route("Common")]
    public class CommonController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();

        [Route("HoTro")]
        public IActionResult HoTro()
        {
            return View();
        }

        [Route("CaiDatTaiKhoan")]
        public IActionResult CaiDatTaiKhoan(int idTK, string layoutType) 
        {
            var tk = db.Taikhoans.FirstOrDefault(x => x.IdTk == idTK);
            ViewBag.Anh = db.Anhdaidiens.FirstOrDefault(x => x.IdTk == idTK);
            var layout = layoutType;
            if (tk == null)
            {
                return RedirectToAction("TrangLoi", "Home");
            }
            if (tk != null)
            {
                // Include các dữ liệu liên quan nếu cần thiết
                db.Entry(tk).Reference(x => x.IdLtkNavigation).Load();
                if (layout == "Layout1")
                {
                    ViewBag.layoutType = "Layout1";
                    return View("CaiDatTaiKhoanNTD", tk); // Sử dụng layout 1
                }
                else
                {
                    ViewBag.layoutType = "Layout2";
                    return View("CaiDatTaiKhoanNTV", tk); // Sử dụng layout 2
                }
            }
            //var Id_TK = IdTK;
            //HttpContext.Session.SetInt32("IdTk", Id_TK);
            return View(tk);
        }

        [Route("SuaTaiKhoanCommon")]
        [HttpGet]
        public IActionResult SuaTaiKhoanCommon(int idTK, string layoutType)
        {
            TempData["Message"] = "";
            var tk = db.Taikhoans.FirstOrDefault(x => x.IdTk == idTK);
            var layout = layoutType;
            if (tk == null)
            {
                NotFound();
            }
            ViewBag.IdLtk = new SelectList(db.Loaitaikhoans.ToList(), "IdLtk", "Tenltk");
            if (layout == "Layout1")
            {
                ViewBag.layoutType = "Layout1";
                return View("SuaTaiKhoanNTD", tk);
            }
            else
            {
                ViewBag.layoutType = "Layout2";
                return View("SuaTaiKhoanNTV", tk); 
            }
        }

        [Route("SuaTaiKhoanCommon")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaTaiKhoanCommon(Taikhoan tk)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tk).State = EntityState.Modified;
                db.SaveChanges();

                // Trả về một JSON object chứa thông điệp thành công và loại thông điệp
                HttpContext.Session.SetString("Hoten", tk.Hoten);
                return Json(new { messageType = "success", message = "Sửa thành công" });
            }
            else
            {
                // Trả về một JSON object chứa thông điệp lỗi và loại thông điệp
                return Json(new { messageType = "error", message = "Vui lòng kiểm tra lại thông tin" });
            }
        }


        //[Route("SuaTaiKhoanCommon")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult SuaTaiKhoanCommon(Taikhoan tk, string layoutType)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(tk).State = EntityState.Modified;
        //        db.SaveChanges();
        //        //ViewBag.IdTk = tk.IdTk;
        //        //ViewBag.layoutType = layoutType;
        //        // Truyền thông điệp thành công trực tiếp tới view
        //        ViewBag.MessageType = "success";
        //        ViewBag.Message = "Sửa thành công";
        //        //return View("SuaTaiKhoanCommon", "Common", new { idTK = tk.IdTk, layoutType = layoutType });
        //    }
        //    else
        //    {
        //        // Truyền thông điệp thất bại trực tiếp tới view
        //        ViewBag.MessageType = "error";
        //        ViewBag.Message = "Vui lòng kiểm tra lại thông tin";
        //    }
        //    ViewBag.IdLtk = new SelectList(db.Loaitaikhoans.ToList(), "IdLtk", "Tenltk");
        //    return RedirectToAction("SuaTaiKhoanCommon", "Common", new { idTK = tk.IdTk, layoutType = layoutType });
        //}

        [Route("SuaAnhDDCommon")]
        [HttpGet]
        public IActionResult SuaAnhDDCommon(int idTK)
        {
            var tk = db.Taikhoans.FirstOrDefault(x => x.IdTk == idTK);

            if (tk != null)
            {
                // Include các dữ liệu liên quan nếu cần thiết
                db.Entry(tk).Reference(x => x.IdLtkNavigation).Load();
                return View(tk);
            }
            var Id_TK = idTK;
            HttpContext.Session.SetInt32("IdTk", Id_TK);
            return View(tk);
        }

        [Route("SuaAnhDDCommon")]
        [HttpPost]
        public IActionResult SuaAnhDDCommon(Taikhoan tk)
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");

            if (tk != null)
            {
                // Include các dữ liệu liên quan nếu cần thiết
                db.Entry(tk).Reference(x => x.IdLtkNavigation).Load();
                return View(tk);
            }
            //var Id_TK = IdTK;
            //HttpContext.Session.SetInt32("IdTk", Id_TK);
            return View(tk);
        }
    }
}
