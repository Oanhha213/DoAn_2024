using DoAnNet7_2.Models;
using DoAnNet7_2.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

namespace DoAnNet7_2.Controllers
{
    [Route("NguoiTimViec")]
    public class NguoiTimViecController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();

        //[Authentication]
        [Route("")]
        [Route("NguoiTimViec")]
        [HttpGet]
        public IActionResult NguoiTimViec(int? page, string searchTerm, string luong, string tinhthanh, string nganhnghe, string kinhnghiem)
        {

            int pageNumber = page ?? 1;
            int pageSize = 8;

            var query = db.Baituyendungs.Where(x => x.IdTtht == 2)
                        .Include(x => x.IdTkNavigation)
                        .Include(x => x.IdNnNavigation)
                        .Include(x => x.IdLcvNavigation)
                        .Include(x => x.IdLuongNavigation)
                        .Include(x => x.IdTthtNavigation)
                        .Include(x => x.IdTtNavigation)
                        .Include(x => x.IdTgknNavigation)
                        .AsNoTracking()
                        .OrderBy(x => x.IdBtd);

            // Kiểm tra xem có giá trị searchTerm không rồi gán giá trị cho ViewBag.SearchTerm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.searchTerm = searchTerm;
                query = (IOrderedQueryable<Baituyendung>)query
                    .Where(x => x.Tencongviec.Contains(searchTerm) || x.IdNnNavigation.Tennganhnghe.Contains(searchTerm)
                    || x.IdLcvNavigation.Tenlcv.Contains(searchTerm)
                    || x.IdLuongNavigation.Tenmucluong.Contains(searchTerm)
                    || x.IdTtNavigation.Tentt.Contains(searchTerm)
                    );
            }
            // Truy vấn dữ liệu từ cơ sở dữ liệu
            //var luongs = db.Luongs.Select(x => x.Tenmucluong).Distinct().OrderBy(x => x).ToList();
            var tinhthanhs = db.Tinhthanhs.Select(x => x.Tentt).Distinct().ToList();
            var nganhnghes = db.Nganhnghes.Select(x => x.Tennganhnghe).Distinct().ToList();
            //var kinhnghiems = db.Thoigiankinhnghiems.Select(x => x.Tentgkn).Distinct().ToList();

            // Kiểm tra xem có bản ghi nào có Tennganhnghe hoặc Tentt là "Khác" không
            var khacNganhNghe = nganhnghes.FirstOrDefault(x => x == "Khác");
            var khacTinhThanh = tinhthanhs.FirstOrDefault(x => x == "Khác");

            if (khacNganhNghe != null)
            {
                nganhnghes.Remove(khacNganhNghe);
                nganhnghes.Add(khacNganhNghe);
            }

            if (khacTinhThanh != null)
            {
                tinhthanhs.Remove(khacTinhThanh);
                tinhthanhs.Add(khacTinhThanh);
            }
            // Tạo các option từ dữ liệu truy vấn
            //ViewBag.luongOptions = luongs.Select(l => new SelectListItem { Value = l, Text = l }).ToList();
            ViewBag.tinhthanhOptions = tinhthanhs.Select(tt => new SelectListItem { Value = tt, Text = tt }).ToList();
            ViewBag.nganhngheOptions = nganhnghes.Select(nn => new SelectListItem { Value = nn, Text = nn }).ToList();
            //ViewBag.kinhnghiemOptions = kinhnghiems.Select(kn => new SelectListItem { Value = kn, Text = kn }).ToList();

            // Áp dụng các bộ lọc
            // Áp dụng các bộ lọc
            if (!string.IsNullOrEmpty(luong) && luong != "Mức lương")
            {
                query = (IOrderedQueryable<Baituyendung>)query.Where(x => x.IdLuongNavigation.Tenmucluong == luong);
                ViewBag.luong = luong;
            }

            if (!string.IsNullOrEmpty(tinhthanh) && tinhthanh != "Tỉnh thành")
            {
                query = (IOrderedQueryable<Baituyendung>)query.Where(x => x.IdTtNavigation.Tentt == tinhthanh);
                ViewBag.tinhthanh = tinhthanh;
            }

            if (!string.IsNullOrEmpty(nganhnghe) && nganhnghe != "Ngành nghề")
            {
                query = (IOrderedQueryable<Baituyendung>)query.Where(x => x.IdNnNavigation.Tennganhnghe == nganhnghe);
                ViewBag.nganhnghe = nganhnghe;
            }

            if (!string.IsNullOrEmpty(kinhnghiem) && kinhnghiem != "Kinh nghiệm")
            {
                query = (IOrderedQueryable<Baituyendung>)query.Where(x => x.IdTgknNavigation.Tentgkn == kinhnghiem);
                ViewBag.kinhnghiem = kinhnghiem;
            }
            else
            {
                ViewBag.searchTerm = null; // Đặt ViewBag.SearchTerm về null nếu không có searchTerm
                ViewBag.luong = null;
                ViewBag.tinhthanh = null;
                ViewBag.nganhnghe = null;
                ViewBag.kinhnghiem = null;
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
                return PartialView("_NguoiTimViecPartial", pagedList);
            }

            return View(pagedList);
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
