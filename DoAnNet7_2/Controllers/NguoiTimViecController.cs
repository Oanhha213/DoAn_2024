using DoAnNet7_2.Models;
using DoAnNet7_2.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace DoAnNet7_2.Controllers
{
    [Route("NguoiTimViec")]
    public class NguoiTimViecController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();

        //[Authentication]
        [Route("")]
        [Route("NguoiTimViec")]
        public IActionResult NguoiTimViec()
        {
            var idTK = HttpContext.Session.GetInt32("IdTk");
            ViewBag.Hoten = db.Taikhoans.FirstOrDefault(x => x.IdTk == idTK)?.Hoten;
            return View();
        }
        
        [Route("DanhSachCV")]
        public IActionResult DanhSachCV()
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");
            if (!ID_TK.HasValue)
            {
                // Xử lý trường hợp người dùng không hợp lệ, có thể chuyển hướng hoặc xử lý lỗi khác
                return RedirectToAction("TrangLoi", "Home");
            }
            var lstCV = db.Soyeulyliches.Where(x => x.IdTk == ID_TK).ToList();
            return View(lstCV);
        }

        [Route("ThemCV")]
        [HttpGet]
        public IActionResult ThemCV()
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");

            if (!ID_TK.HasValue)
            {
                // Xử lý trường hợp người dùng không hợp lệ, có thể chuyển hướng hoặc xử lý lỗi khác
                return RedirectToAction("TrangLoi", "Home");
            }

            var cv = new Soyeulylich
            {
                IdTk = ID_TK.Value
            };

            return View(cv);
        }

        [Route("ThemCV")]
        [HttpPost]
        public IActionResult ThemCV(Soyeulylich syll, IFormFile cvFile)
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");

            if (!ID_TK.HasValue)
            {
                // Xử lý trường hợp người dùng không hợp lệ, có thể chuyển hướng hoặc xử lý lỗi khác
                return RedirectToAction("TrangLoi", "Home");
            }
            if (cvFile != null && cvFile.Length > 0)
            {
                // Tạo tên tệp duy nhất dựa trên thời gian hiện tại
                var fileName = $"{DateTime.Now.Ticks}_CV.pdf";

                // Đường dẫn tới thư mục lưu trữ tệp
                var uploadDirectory = Path.Combine("uploads", "CV");

                // Tạo thư mục nếu nó không tồn tại
                var absoluteUploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uploadDirectory);
                if (!Directory.Exists(absoluteUploadDirectory))
                {
                    Directory.CreateDirectory(absoluteUploadDirectory);
                }

                // Đường dẫn đầy đủ tới tệp PDF
                var filePath = Path.Combine(uploadDirectory, fileName);

                // Lưu tệp vào thư mục uploads
                using (var stream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath), FileMode.Create))
                {
                    cvFile.CopyTo(stream);
                }

                if (ModelState.IsValid)
                {
                    syll.IdTk = ID_TK.Value;
                    syll.Duongdansyll = filePath;
                    db.Soyeulyliches.Add(syll);
                    db.SaveChanges();
                    return RedirectToAction("DanhSachCV", "NguoiTimViec");
                }
            }
            return View(syll);
        }

        public IActionResult ViewCV(int syllId)
        {
            var syll = db.Soyeulyliches.FirstOrDefault(x => x.IdSyll == syllId);
            if (syll == null)
            {
                return NotFound("Không tìm thấy CV.");
            }

            // Kiểm tra xem tệp có tồn tại không
            if (!System.IO.File.Exists(syll.Duongdansyll))
            {
                return NotFound("Không tìm thấy tệp CV.");
            }

            // Trả về FileResult để hiển thị tệp PDF
            var fileContent = System.IO.File.ReadAllBytes(syll.Duongdansyll);
            return File(fileContent, "application/pdf");
        }
    }
}
