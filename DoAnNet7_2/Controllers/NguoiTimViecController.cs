using DoAnNet7_2.Models;
using DoAnNet7_2.Models.Authentication;
using DoAnNet7_2.Services;
using Microsoft.AspNetCore.Http;
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
            var ID_TK = HttpContext.Session.GetInt32("IdTk");
            int pageNumber = page ?? 1;
            int pageSize = 8;
            // Tạo danh sách các đối tượng JobStatus
            List<TTCongViec> jobStatuses = new List<TTCongViec>();
            DateTime now = DateTime.Now;
            var query = db.Baituyendungs.Where(x => x.IdTtht == 2)
                        .Include(x => x.IdTkNavigation)
                        .Include(x => x.IdNnNavigation)
                        .Include(x => x.IdLcvNavigation)
                        .Include(x => x.IdLuongNavigation)
                        .Include(x => x.IdTthtNavigation)
                        .Include(x => x.IdTtNavigation)
                        .Include(x => x.IdTgknNavigation)
                        .OrderBy(x => x.Hannopcv < DateTime.Now) // Sắp xếp các bài tuyển dụng đã hết hạn xuống cuối danh sách
                        .ThenBy(x => x.IdBtd); // Sau đó, sắp xếp theo thứ tự bình thường
            var items = query.ToList();
            foreach (var item in items)
            {
                // Kiểm tra các trạng thái và gán cho đối tượng JobStatus tương ứng
                bool isExpired = (item.Hannopcv < now);
                bool isApplied = db.CongviecSylls.Any(x => x.IdSyllNavigation.IdTk == ID_TK && x.IdBtd == item.IdBtd);
                bool isLiked = db.Thichcongviecs.Any(x => x.IdTk == ID_TK && x.IdBtd == item.IdBtd);

                TTCongViec status = new TTCongViec
                {
                    IdBtd = item.IdBtd,
                    TTUT = isApplied ? "Đã ứng tuyển" : "Chưa ứng tuyển",
                    TTThich = isLiked ? "Đã thích" : "Chưa thích",
                    TTHH = isExpired ? "Đã hết hạn" : "Chưa hết hạn"
                };

                // Thêm đối tượng JobStatus vào danh sách
                jobStatuses.Add(status);
            }

            // Gán danh sách JobStatus vào ViewBag
            ViewBag.JobStatuses = jobStatuses;

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
            var tinhthanhs = db.Tinhthanhs.Select(x => x.Tentt).Distinct().ToList();
            var nganhnghes = db.Nganhnghes.Select(x => x.Tennganhnghe).Distinct().ToList();

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

            ViewBag.tinhthanhOptions = tinhthanhs.Select(tt => new SelectListItem { Value = tt, Text = tt }).ToList();
            ViewBag.nganhngheOptions = nganhnghes.Select(nn => new SelectListItem { Value = nn, Text = nn }).ToList();

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

            if (ID_TK != null)
            {
                HttpContext.Session.SetInt32("IdTk", (int)ID_TK);
            }
            ViewBag.IdTk = ID_TK;
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
            HttpContext.Session.SetInt32("IdTk", (int)ID_TK);
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
            HttpContext.Session.SetInt32("IdTk", (int)ID_TK);

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
                HttpContext.Session.SetInt32("IdTk", (int)ID_TK);

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

        [Route("ViewCV")]
        [HttpGet]
        public IActionResult ViewCV(int idCV)
        {
            var syll = db.Soyeulyliches.FirstOrDefault(x => x.IdSyll == idCV);
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

        [Route("TaiKhoanNTV")]
        public IActionResult TaiKhoanNTV()
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");
            var TenanhDD = HttpContext.Session.GetString("Tenanh");
            var tk = db.Taikhoans.FirstOrDefault(x => x.IdTk == ID_TK);
            Debug.WriteLine("IdTk: " + ID_TK);
            ViewBag.Anh = db.Anhdaidiens.FirstOrDefault(x => x.IdTk == ID_TK);
            if (tk == null)
            {
                return RedirectToAction("TrangLoi", "Home");
            }
            if (tk != null)
            {
                if (ID_TK.HasValue)
                {
                    HttpContext.Session.SetInt32("IdTk", (int)ID_TK);
                }
                // Include các dữ liệu liên quan nếu cần thiết
                db.Entry(tk).Reference(x => x.IdLtkNavigation).Load();
                return View(tk);
            }
            //var Id_TK = IdTK;
            //HttpContext.Session.SetInt32("IdTk", Id_TK);
            return View(tk);
        }

        [Route("SuaTaiKhoanNTV")]
        [HttpGet]
        public IActionResult SuaTaiKhoanNTV(int idTK)
        {
            TempData["Message"] = "";
            var tk = db.Taikhoans.FirstOrDefault(x => x.IdTk == idTK);
            var ID_TK = idTK;
            HttpContext.Session.SetInt32("IdTk", ID_TK);
            if (tk == null)
            {
                NotFound();
            }
            ViewBag.IdLtk = new SelectList(db.Loaitaikhoans.ToList(), "IdLtk", "Tenltk");
            return View(tk);
        }

        [Route("SuaTaiKhoanNTV")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaTaiKhoanNTV(Taikhoan tk)
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");
            if (ID_TK.HasValue)
            {
                HttpContext.Session.SetInt32("IdTk", (int)ID_TK);
            }
            if (ModelState.IsValid)
            {

                db.Entry(tk).State = EntityState.Modified;
                //var IdTk = tk.IdTk;
                //HttpContext.Session.SetInt32("IdTk", IdTk);
                db.SaveChanges();
                HttpContext.Session.SetString("Hoten", tk.Hoten);
                // Truyền thông điệp thành công trực tiếp tới view
                ViewBag.MessageType = "success";
                ViewBag.Message = "Sửa thành công";
            }
            else
            {
                // Truyền thông điệp thất bại trực tiếp tới view
                ViewBag.MessageType = "error";
                ViewBag.Message = "Vui lòng kiểm tra lại thông tin";
            }
            ViewBag.IdLtk = new SelectList(db.Loaitaikhoans.ToList(), "IdLtk", "Tenltk");
            return View(tk);
        }

        [Route("SuaAnhDDNTV")]
        [HttpGet]
        public IActionResult SuaAnhDDNTV(int idTK)
        {
            var anhDD = db.Anhdaidiens.FirstOrDefault(a => a.IdTk == idTK);
            var Id_TK = idTK;
            if (anhDD == null)
            {
                return RedirectToAction("UpAnhDaiDienNTD", "NhaTuyenDung");
            }

            HttpContext.Session.SetInt32("IdTk", Id_TK);
            return View(anhDD);
        }

        [Route("SuaAnhDDNTV")]
        [HttpPost]
        public IActionResult SuaAnhDDNTV(Anhdaidien anhDD)
        {
            var existingAnhDD = db.Anhdaidiens.FirstOrDefault(a => a.IdTk == anhDD.IdTk);
            var ID_TK = HttpContext.Session.GetInt32("IdTk");
            var idTK = ID_TK;
            if (existingAnhDD == null)
            {
                return NotFound();
            }

            if (anhDD.UpAnhDaiDien != null && ModelState.IsValid)
            {
                // Xóa ảnh cũ nếu cần

                // Lưu ảnh mới
                existingAnhDD.Tenanh = LuuAnh.LuuAnhDaiDien(anhDD.UpAnhDaiDien);

                // Cập nhật thông tin ảnh đại diện trong cơ sở dữ liệu
                db.SaveChanges();
                if (idTK != null)
                {
                    HttpContext.Session.SetInt32("IdTk", (int)idTK);
                    HttpContext.Session.SetString("Tenanh", existingAnhDD.Tenanh);
                }
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
            HttpContext.Session.SetString("Tenanh", existingAnhDD.Tenanh);
            return View(existingAnhDD);
        }

        [Route("DSCVNTV")]
        [HttpGet]
        public IActionResult DSCVNTV(int idTK, int? page, string searchTerm)
        {
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            int pageSize = 4;
            var HT = db.Taikhoans.FirstOrDefault(x => x.IdTk == idTK)?.Hoten;
            var query = db.Soyeulyliches.Where(x => x.IdTk == idTK)
                        .AsNoTracking()
                        .OrderBy(x => x.IdSyll);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.searchTerm = searchTerm;
                Debug.WriteLine("SearchTerm: " + searchTerm);

                query = (IOrderedQueryable<Soyeulylich>)query.Where(x => x.Tensyll.Contains(searchTerm));
                // Duyệt qua từng phần tử trong query và in ra thông tin của mỗi phần tử
                //foreach (var item in query)
                //{
                //    // In ra các thông tin mà bạn muốn hiển thị
                //    Debug.WriteLine("Id: " + item.IdSyll); // Ví dụ: Id là thuộc tính của Soyeulylich
                //    Debug.WriteLine("Ten: " + item.Tensyll); // Ví dụ: Tensyll là thuộc tính của Soyeulylich
                //                                             // Thêm các thông tin khác nếu cần
                //}
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
                // In ra các thông tin của từng phần tử trong pagedList
                foreach (var item in pagedList)
                {
                    // In ra các thông tin mà bạn muốn hiển thị
                    Debug.WriteLine("Id: " + item.IdSyll); // Ví dụ: Id là thuộc tính của Soyeulylich
                    Debug.WriteLine("Ten: " + item.Tensyll); // Ví dụ: Tensyll là thuộc tính của Soyeulylich
                                                             // Thêm các thông tin khác nếu cần
                }

                return PartialView("_DSCVNTVPartial", pagedList);
            }
            ViewBag.Ho_Ten = HT;
            ViewBag.IdTk = idTK;
            var Id_TK = idTK;
            HttpContext.Session.SetInt32("IdTk", Id_TK);
            return View(pagedList);
        }
        [Route("ThemCVTKNTV")]
        [HttpGet]
        public IActionResult ThemCVTKNTV()
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");
            ViewBag.IdTk = ID_TK;
            if (!ID_TK.HasValue)
            {
                // Xử lý trường hợp người dùng không hợp lệ, có thể chuyển hướng hoặc xử lý lỗi khác
                return RedirectToAction("TrangLoi", "Home");
            }
            HttpContext.Session.SetInt32("IdTk", (int)ID_TK);
            var cv = new Soyeulylich
            {
                IdTk = ID_TK.Value
            };

            return View(cv);
        }

        [Route("ThemCVTKNTV")]
        [HttpPost]
        public IActionResult ThemCVTKNTV(Soyeulylich syll, IFormFile cvFile)
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
                if (ID_TK.HasValue)
                {
                    HttpContext.Session.SetInt32("IdTk", (int)ID_TK);
                }
                if (ModelState.IsValid)
                {
                    syll.IdTk = ID_TK.Value;
                    syll.Duongdansyll = filePath;
                    db.Soyeulyliches.Add(syll);
                    db.SaveChanges();
                    return RedirectToAction("DSCVNTV", "NguoiTimViec", new { idTK = ID_TK.Value });
                }
            }
            return View(syll);
        }

        [Route("SuaCVNTV")]
        [HttpGet]
        public IActionResult SuaCVNTV(int idCV)
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");
            if (ID_TK.HasValue)
            {
                HttpContext.Session.SetInt32("IdTk", (int)ID_TK);
            }
            var syll = db.Soyeulyliches.FirstOrDefault(x => x.IdSyll == idCV);
            return View(syll);
        }

        [Route("SuaCVNTV")]
        [HttpPost]

        public IActionResult SuaCVNTV(Soyeulylich syll)
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");
            if (ID_TK.HasValue)
            {
                HttpContext.Session.SetInt32("IdTk", (int)ID_TK);
            }
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

        [Route("XoaCVNTV")]
        [HttpGet]
        public IActionResult XoaCVNTV(int idCV, int IdTK)
        {
            TempData["Message"] = "";
            var syll = db.Soyeulyliches.Find(idCV);
            var cvsyll = db.CongviecSylls.Where(x => x.IdSyll == idCV).ToList();
            HttpContext.Session.SetInt32("IdTk", IdTK);
            if (cvsyll.Count() > 0)
            {
                TempData["Message"] = $"Không xoá được CV này vì đã ứng tuyển";
                TempData["MessageType"] = "error";
                return RedirectToAction("DSCVNTV", "NguoiTimViec", new { idTK = IdTK });
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
            Debug.WriteLine("IdTk: " + IdTK);
            return RedirectToAction("DSCVNTV", "NguoiTimViec", new { idTK = IdTK });
        }

        [Route("BaiVietDaThichNTV")]
        public IActionResult BaiVietDaThichNTV(int idTK, int? page, string searchTerm)
        {
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            int pageSize = 8;
            var query = db.Thichcongviecs.Where(x => x.IdTk == idTK)
                        .Include(x => x.IdBtdNavigation)
                        .Include(x => x.IdBtdNavigation.IdLcvNavigation)
                        .Include(x => x.IdBtdNavigation.IdLuongNavigation)
                        .Include(x => x.IdBtdNavigation.IdNnNavigation)
                        .Include(x => x.IdBtdNavigation.IdTtNavigation)
                        .Include(x => x.IdBtdNavigation.IdTgknNavigation)
                        .AsNoTracking()
                        .OrderBy(x => x.IdTcv);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.searchTerm = searchTerm;
                query = (IOrderedQueryable<Thichcongviec>)query
                    .Where(x => x.IdBtdNavigation.Tencongviec.Contains(searchTerm) ||
                    x.IdBtdNavigation.IdNnNavigation.Tennganhnghe.Contains(searchTerm) ||
                    x.IdBtdNavigation.IdLcvNavigation.Tenlcv.Contains(searchTerm) ||
                    x.IdBtdNavigation.Diachi.Contains(searchTerm) ||
                    x.IdBtdNavigation.IdTtNavigation.Tentt.Contains(searchTerm));
            }
            else
            {
                ViewBag.searchTerm = null; // Đặt ViewBag.SearchTerm về null nếu không có searchTerm
            }
            HttpContext.Session.SetInt32("IdTk", idTK);

            var pagedList = query.OrderBy(x => x.IdTcv).ToPagedList(pageNumber, pageSize);

            ViewBag.IdTk = idTK;
            ViewBag.Hoten = db.Taikhoans.FirstOrDefault(x => x.IdTk == idTK)?.Hoten;
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
                return PartialView("_BaiVietDaThichNTVPartial", pagedList);
            }
            return View(pagedList);
        }
        [Route("XoaBTDDaThichNTV")]
        [HttpGet]
        public IActionResult XoaBTDDaThichNTV(int idTCV, int idTK)
        {
            TempData["Message"] = "";
            var tcv = db.Thichcongviecs.Find(idTCV);
            HttpContext.Session.SetInt32("IdTk", idTK);

            if (tcv != null)
            {
                db.Thichcongviecs.Remove(tcv);
                db.SaveChanges();
                TempData["Message"] = $"Xoá bài viết đã thích thành công";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = $"Không tìm thấy";
                TempData["MessageType"] = "error";
            }
            return RedirectToAction("BaiVietDaThichNTV", "NguoiTimViec", new { idTK = idTK });
        }

        [Route("BTDDaUngTuyen")]
        [HttpGet]
        public IActionResult BTDDaUngTuyen(int idTK, int? page, string searchTerm)
        {
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            int pageSize = 8;
            var query = db.CongviecSylls.Where(x => x.IdSyllNavigation.IdTk == idTK)
                        .Include(x => x.IdBtdNavigation)
                        .Include(x => x.IdBtdNavigation.IdLcvNavigation)
                        .Include(x => x.IdBtdNavigation.IdLuongNavigation)
                        .Include(x => x.IdBtdNavigation.IdTtNavigation)
                        .Include(x => x.IdBtdNavigation.IdTgknNavigation)
                        .Include(x => x.IdBtdNavigation.IdNnNavigation)
                        .Include(x => x.IdSyllNavigation)
                        .AsNoTracking()
                        .OrderBy(x => x.IdCvsyll);
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
            ViewBag.IdTk = idTK;
            var cvsyll = db.CongviecSylls.Where(x => x.IdSyllNavigation.IdTk == idTK);
            ViewBag.Hoten = db.Taikhoans.FirstOrDefault(x => x.IdTk == idTK)?.Hoten;
            HttpContext.Session.SetInt32("IdTk", idTK);

            // Sắp xếp dữ liệu trước khi phân trang
            query = query.OrderBy(x => x.IdBtdNavigation.Tencongviec);

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
                return PartialView("_BTDDaUngTuyenPartial", pagedList);
            }
            return View(pagedList);
        }

        [Route("ChiTietBTDNTV")]
        public IActionResult ChiTietBTDNTV(int idBTD, int idTK)
        {
            var btd = db.Baituyendungs.FirstOrDefault(x => x.IdBtd == idBTD);
            var listCV = db.Soyeulyliches.Where(cv => cv.IdTk == idTK).ToList();

            ViewBag.ListCV = listCV;
            ViewBag.IdTk = idTK;
            HttpContext.Session.SetInt32("IdTk", idTK);

            if (btd != null)
            {

                var idTKBtd = btd.IdTk;
                var ct = db.Congties.Where(x => x.IdTk == idTKBtd).Include(x => x.IdTtNavigation);
                if (ct != null)
                {
                    var tenct = ct.FirstOrDefault(x => x.IdTk == idTKBtd)?.Tencongty;
                    var logo = ct.FirstOrDefault(x => x.IdTk == idTKBtd)?.Logo;
                    var diachi = ct.FirstOrDefault(x => x.IdTk == idTKBtd)?.Diachi;
                    var tt = ct.FirstOrDefault(x => x.IdTk == idTKBtd)?.IdTtNavigation.Tentt;
                    var sonv = ct.FirstOrDefault(x => x.IdTk == idTKBtd)?.Sonhanvien;
                    ViewBag.Logo = logo;
                    ViewBag.Tenct = tenct;
                    ViewBag.Diachi = diachi;
                    ViewBag.Tinhthanh = tt;
                    ViewBag.SoNV = sonv;
                    ViewBag.congty = ct;
                }
                else
                {
                    ViewBag.congty = null;
                }
                // Kiểm tra trạng thái ứng tuyển và thích của người dùng
                bool isApplied = db.CongviecSylls.Any(x => x.IdSyllNavigation.IdTk == idTK && x.IdBtd == idBTD);
                bool isLiked = db.Thichcongviecs.Any(x => x.IdTk == idTK && x.IdBtd == idBTD);

                ViewBag.TTUT = isApplied ? "Đã ứng tuyển" : "Chưa ứng tuyển";
                ViewBag.TTThich = isLiked ? "Đã thích" : "Chưa thích";
                ViewBag.TTHH = (btd.Hannopcv < DateTime.Now) ? "Đã hết hạn" : "Chưa hết hạn";
                ViewBag.Tencongviec = btd.Tencongviec;
                ViewBag.IdBtd = idBTD;
                db.Entry(btd).Reference(x => x.IdTgknNavigation).Load();
                db.Entry(btd).Reference(x => x.IdLcvNavigation).Load();
                db.Entry(btd).Reference(x => x.IdNnNavigation).Load();
                db.Entry(btd).Reference(x => x.IdTkNavigation).Load();
                db.Entry(btd).Reference(x => x.IdTtNavigation).Load();
                db.Entry(btd).Reference(x => x.IdLuongNavigation).Load();
                db.Entry(btd).Reference(x => x.IdTthtNavigation).Load();
            }
            return View(btd);
        }

        [HttpPost]
        [Route("UngTuyenNTV")]
        public IActionResult UngTuyenNTV(int idTK, int idBTD, int idCV)
        {
            // Kiểm tra xem idTK, idBTD, idCV có tồn tại và hợp lệ không
            var tkExists = db.Taikhoans.Any(t => t.IdTk == idTK);
            var btdExists = db.Baituyendungs.Any(b => b.IdBtd == idBTD);
            var cvExists = db.Soyeulyliches.Any(cv => cv.IdSyll == idCV && cv.IdTk == idTK);
            ViewBag.IdBtd = idBTD;
            ViewBag.IdTk = idTK;
            HttpContext.Session.SetInt32("IdTk", idTK);

            if (!tkExists || !btdExists || !cvExists)
            {
                // Trả về một kết quả tùy thuộc vào yêu cầu của bạn, ví dụ: BadRequest hoặc NotFound
                return BadRequest("Thông tin không hợp lệ.");
            }

            // Tạo một bản ghi mới trong bảng CongviecSyll
            var newCongviecSyll = new CongviecSyll
            {
                IdBtd = idBTD,
                IdSyll = idCV
            };

            // Thêm bản ghi mới vào cơ sở dữ liệu
            db.CongviecSylls.Add(newCongviecSyll);
            db.SaveChanges();

            // Cập nhật trạng thái của công việc
            var btd = db.Baituyendungs.FirstOrDefault(b => b.IdBtd == idBTD);
            if (btd != null)
            {
                ViewBag.TTUT = "Đã ứng tuyển";
            }
            // Trả về một phản hồi OK để cho biết rằng ứng tuyển thành công
            //return Ok("Ứng tuyển thành công!");
            //return RedirectToAction("ChiTietBTDNTV", "NguoiTimViec", new { idTK = idTK });
            return Json(new { success = true, message = "Ứng tuyển thành công!" });

        }

        [HttpPost]
        public IActionResult ThichCongViec(int idBTD, int idTK) // Thêm tham số idTK vào phương thức
        {
            ViewBag.IdBtd = idBTD;
            ViewBag.IdTk = idTK;
            // Kiểm tra xem idBTD và idTK đã tồn tại trong bảng ThichCongViec chưa
            var thichCV = db.Thichcongviecs.FirstOrDefault(tc => tc.IdBtd == idBTD && tc.IdTk == idTK);
            HttpContext.Session.SetInt32("IdTk", idTK);

            if (thichCV == null)
            {
                // Nếu chưa tồn tại, thêm mới vào bảng ThichCongViec
                db.Thichcongviecs.Add(new Thichcongviec { IdBtd = idBTD, IdTk = idTK });
                db.SaveChanges();
                return Json(new { success = true, thich = true });
            }
            else
            {
                // Nếu đã tồn tại, xoá bản ghi trong bảng ThichCongViec
                db.Thichcongviecs.Remove(thichCV);
                db.SaveChanges();
                return Json(new { success = true, thich = false });
            }
        }
    }
}
