using DoAnNet7_2.Models;
using DoAnNet7_2.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DoAnNet7_2.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("CV")]
    public class CVController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();

        [Authentication]
        [Route("DanhSachCV")]
        [HttpGet]
        public IActionResult DanhSachCV(int? page, string searchTerm)
        {
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            int pageSize = 12;
            var query = db.Soyeulyliches.Include(x => x.IdTkNavigation)
                            .AsNoTracking().OrderBy(x => x.IdTkNavigation.Email);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.searchTerm = searchTerm;
                query = (IOrderedQueryable<Soyeulylich>)query.Where(x => x.Tensyll.Contains(searchTerm) ||
                                                            x.IdTkNavigation.Hoten.Contains(searchTerm) ||
                                                            x.IdTkNavigation.Email.Contains(searchTerm));
            }
            else
            {
                ViewBag.searchTerm = null; // Đặt ViewBag.SearchTerm về null nếu không có searchTerm
            }
            // Sắp xếp dữ liệu trước khi phân trang
            query = query.OrderBy(x => x.Tensyll);

            var pagedList = query.ToPagedList(pageNumber, pageSize);
            // Kiểm tra xem có kết quả trả về hay không
            if (pagedList.Count == 0)
            {
                ViewBag.NoResultsMessage = "Không có kết quả phù hợp.";
            }
            else
            {
                ViewBag.NoResultsMessage = null;
            }
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DSCVPartial", pagedList);
            }
            return View(pagedList);
        }

        [Authentication]
        [HttpGet]
        [Route("DSCVUngVien")]
        public IActionResult DSCVUngVien(int IdTK, int? page, string searchTerm)
        {
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            int pageSize = 4;
            var HT = db.Taikhoans.FirstOrDefault(x => x.IdTk == IdTK)?.Hoten;
            var query = db.Soyeulyliches.Where(x => x.IdTk == IdTK)
                        .AsNoTracking()
                        .OrderBy(x => x.IdSyll);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.SearchTerm = searchTerm;
                query = (IOrderedQueryable<Soyeulylich>)query.Where(x => x.Tensyll.Contains(searchTerm));
            }
            else
            {
                ViewBag.SearchTerm = null; // Đặt ViewBag.SearchTerm về null nếu không có searchTerm
            }
            // Sắp xếp dữ liệu trước khi phân trang
            query = query.OrderBy(x => x.Tensyll);
            ViewBag.Ho_Ten = HT;
            ViewBag.IdTK = IdTK;
            var Id_TK = IdTK;
            HttpContext.Session.SetInt32("IdTk", Id_TK);

            var pagedList = query.ToPagedList(pageNumber, pageSize);
            // Kiểm tra xem có kết quả trả về hay không
            if (pagedList.Count == 0)
            {
                ViewBag.NoResultsMessage = "Không có kết quả phù hợp.";
            }
            else
            {
                ViewBag.NoResultsMessage = null;
            }
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_CVUngVienPartial", pagedList);
            }
            
            return View(pagedList);
        }

        [Authentication]
        [HttpGet]
        [Route("TaiXuongCV")]
        public IActionResult TaiXuongCV(int idCV)
        {
            var cv = db.Soyeulyliches.FirstOrDefault(x => x.IdSyll == idCV);
            if (cv == null)
            {
                return NotFound("Không tìm thấy CV.");
            }

            // Kiểm tra xem tệp có tồn tại không
            if (!System.IO.File.Exists(cv.Duongdansyll))
            {
                return NotFound("Không tìm thấy tệp CV.");
            }

            // Lấy tên tệp từ đường dẫn
            string fileName = Path.GetFileName(cv.Duongdansyll);

            // Đọc dữ liệu của tệp PDF
            var fileContent = System.IO.File.ReadAllBytes(cv.Duongdansyll);

            // Trả về tệp PDF để tải xuống
            return File(fileContent, "application/pdf", fileName);
        }

        [Authentication]
        [Route("DSBTDCV")]
        public IActionResult DSBTDCV(int idCV, int? page, string searchTerm)
        {
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            int pageSize = 4;
            var query = db.CongviecSylls.Where(x => x.IdSyll == idCV)
                        .Include(x => x.IdBtdNavigation)
                        .Include(x => x.IdBtdNavigation.IdLcvNavigation)
                        .Include(x => x.IdBtdNavigation.IdNnNavigation)
                        .Include(x => x.IdBtdNavigation.IdLuongNavigation)
                        .Include(x => x.IdBtdNavigation.IdTtNavigation)
                        .AsNoTracking()
                        .OrderBy(x => x.IdCvsyll);
            ViewBag.IdSyll = idCV;
            ViewBag.IdTk = db.Soyeulyliches.FirstOrDefault(x => x.IdSyll == idCV)?.IdTk;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.SearchTerm = searchTerm;
                query = (IOrderedQueryable<CongviecSyll>)query
                    .Where(x => x.IdBtdNavigation.Tencongviec.Contains(searchTerm)
                    || x.IdBtdNavigation.IdLcvNavigation.Tenlcv.Contains(searchTerm)
                    || x.IdBtdNavigation.Diachi.Contains(searchTerm)
                    || x.IdBtdNavigation.IdTtNavigation.Tentt.Contains(searchTerm)
                    || x.IdBtdNavigation.IdLuongNavigation.Tenmucluong.Contains(searchTerm));
            }
            else
            {
                ViewBag.SearchTerm = null; // Đặt ViewBag.SearchTerm về null nếu không có searchTerm
            }

            var pagedList = query.ToPagedList(pageNumber, pageSize);
            // Kiểm tra xem có kết quả trả về hay không
            if (pagedList.Count == 0)
            {
                ViewBag.NoResultsMessage = "Không có kết quả phù hợp.";
            }
            else
            {
                ViewBag.NoResultsMessage = null;
            }
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DSBTDCVPartial", pagedList);
            }
            return View(pagedList);
        }

        [Authentication]
        [Route("ThemCVUngVien")]
        [HttpGet]
        public IActionResult ThemCVUngVien()
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");
            ViewBag.IdTk = ID_TK;
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

        [Authentication]
        [Route("ThemCVUngVien")]
        [HttpPost]
        public IActionResult ThemCVUngVien(Soyeulylich syll, IFormFile cvFile)
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
                var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "CV");

                // Tạo thư mục nếu nó không tồn tại
                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                // Đường dẫn đầy đủ tới tệp PDF
                var filePath = Path.Combine(uploadDirectory, fileName);

                // Lưu tệp vào thư mục uploads
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    cvFile.CopyTo(stream);
                }

                if (ModelState.IsValid)
                {
                    syll.IdTk = ID_TK.Value;
                    syll.Duongdansyll = filePath;
                    db.Soyeulyliches.Add(syll);
                    db.SaveChanges();
                    return RedirectToAction("DSCVUngVien", "CV", new { IdTK = ID_TK.Value });
                }
            }
            return View(syll);
        }

        [Route("ViewCV")]
        public IActionResult ViewCV(int idCV)
        {
            var cv = db.Soyeulyliches.FirstOrDefault(x => x.IdSyll == idCV);
            if (cv == null)
            {
                return NotFound("Không tìm thấy CV.");
            }

            // Kiểm tra xem tệp có tồn tại không
            if (!System.IO.File.Exists(cv.Duongdansyll))
            {
                return NotFound("Không tìm thấy tệp CV.");
            }

            // Trả về FileResult để hiển thị tệp PDF
            var fileContent = System.IO.File.ReadAllBytes(cv.Duongdansyll);
            return File(fileContent, "application/pdf");
        }


        //public IActionResult ReturnToPreviousView()
        //{

        //    // Kiểm tra xem TempData có chứa thông tin về đường dẫn trước đó hay không
        //    if (TempData["ReturnUrl"] != null)
        //    {
        //        string returnUrl = TempData["ReturnUrl"].ToString();
        //        Debug.WriteLine("URL: " + returnUrl);
        //        return Redirect(returnUrl);
        //    }
        //    else
        //    {
        //        Debug.WriteLine("URL: " + TempData["ReturnUrl"]);
        //        // Nếu TempData không có thông tin, chuyển hướng đến một view mặc định
        //        return RedirectToAction("DSCVUngVien", "CV");
        //    }
        //}

        [Authentication]
        [Route("ThemCV")]
        [HttpGet]
        public IActionResult ThemCV()
        {
            ViewBag.IdTk = new SelectList(db.Taikhoans.Where(x => x.IdLtk == 3).ToList(), "IdTk", "Email");
            ViewBag.Taikhoan = db.Taikhoans.Where(x => x.IdLtk == 3).ToList();
            return View();
        }

        [Authentication]
        [Route("ThemCV")]
        [HttpPost]
        public IActionResult ThemCV(Soyeulylich syll, IFormFile cvFile)
        {
            if (cvFile != null && cvFile.Length > 0)
            {
                // Tạo tên tệp duy nhất dựa trên thời gian hiện tại
                var fileName = $"{DateTime.Now.Ticks}_CV.pdf";

                // Đường dẫn tới thư mục lưu trữ tệp
                var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "CV");

                // Tạo thư mục nếu nó không tồn tại
                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                // Đường dẫn đầy đủ tới tệp PDF
                var filePath = Path.Combine(uploadDirectory, fileName);

                // Lưu tệp vào thư mục uploads
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    cvFile.CopyTo(stream);
                }

                if (ModelState.IsValid)
                {
                    syll.Duongdansyll = filePath;
                    db.Soyeulyliches.Add(syll);
                    db.SaveChanges();
                    return RedirectToAction("DanhSachCV", "CV");
                }
            }
            ViewBag.IdTk = new SelectList(db.Taikhoans.Where(x => x.IdLtk == 3).ToList(), "IdTk", "Email");
            ViewBag.Taikhoan = db.Taikhoans.Where(x => x.IdLtk == 3).ToList();
            return View(syll);
        }

        [Authentication]
        [Route("SuaCV")]
        [HttpGet]
        public IActionResult SuaCV(int idSYLL)
        {
            var syll = db.Soyeulyliches.FirstOrDefault(x => x.IdSyll == idSYLL);
            return View(syll);
        }

        [Authentication]
        [Route("SuaCV")]
        [HttpPost]

        public IActionResult SuaCV(Soyeulylich syll)
        {
            if (ModelState.IsValid || (!string.IsNullOrEmpty(syll.Tensyll)))
            {
                db.Entry(syll).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.MessageType = "success";
                ViewBag.Message = "Sửa thành công";
                ViewBag.IdTk = syll.IdTk;
                //return RedirectToAction("DSCVUngVien", "CV", new { IdTK = syll.IdTk });
            }
            else
            {
                // Truyền thông điệp thất bại trực tiếp tới view
                ViewBag.MessageType = "error";
                ViewBag.Message = "Vui lòng kiểm tra lại thông tin";
            }
            return View(syll);
        }

        [Authentication]
        [Route("XoaCV")]
        [HttpGet]
        public IActionResult XoaCV(int idCV, int idTK)
        {
            TempData["Message"] = "";
            var syll = db.Soyeulyliches.Find(idCV);
            var cvsyll = db.CongviecSylls.Where(x => x.IdSyll == idCV).ToList();
            if (cvsyll.Count() > 0 )
            {
                TempData["Message"] = $"Không xoá được CV này vì đã ứng tuyển";
                TempData["MessageType"] = "error";
                return RedirectToAction("DSCVUngVien", "CV", new { IdTK = idTK });
            }
            if (syll != null)
            {
                db.Soyeulyliches.Remove(syll);
                db.SaveChanges(); 
                TempData["Message"] = $"Xoá CV thành công";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = $"Không tìm thấy CV có ID {syll}";
                TempData["MessageType"] = "error";
            }
            return RedirectToAction("DSCVUngVien", "CV", new { IdTK = idTK });
        }
    }
}
