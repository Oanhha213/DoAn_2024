
using DoAnNet7_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Diagnostics;
using System.Linq;

namespace DoAnNet7_2.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("TaiKhoan")]
    public class TKNguoiTimViecController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();

        [Route("")]
        [Route("DSNguoiTimViec")]
        [HttpGet]
        public IActionResult DSNguoiTimViec(int? page, string searchTerm)
        {
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            int pageSize = 8;
            var query = db.Taikhoans.Include(tk => tk.IdLtkNavigation) // Bao gồm dữ liệu về Loaitaikhoan
                        .Where(tk => tk.IdLtk == 3)
                        .AsNoTracking()
                        .OrderBy(x => x.IdTk);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.searchTerm = searchTerm;
                query = (IOrderedQueryable<Taikhoan>)query.Where(x => x.Hoten.Contains(searchTerm) || x.Email.Contains(searchTerm) || x.Sdt.Contains(searchTerm));
            }
            else
            {
                ViewBag.searchTerm = null; // Đặt ViewBag.SearchTerm về null nếu không có searchTerm
            }

            var pagedList = query.OrderBy(x => x.IdTk).ToPagedList(pageNumber, pageSize);
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
                return PartialView("_DSNTVPartial", pagedList);
            }
            return View(pagedList);
        }

        [Route("BaiVietDaThich")]
        public IActionResult BaiVietDaThich(int idTK, int? page, string searchTerm)
        {
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            int pageSize = 8;
            var query = db.Thichcongviecs.Where(x => x.IdTk == idTK)
                        .Include(x => x.IdBtdNavigation)
                        .Include(x => x.IdBtdNavigation.IdLcvNavigation)
                        .Include(x => x.IdBtdNavigation.IdLuongNavigation)
                        .Include(x => x.IdBtdNavigation.IdNnNavigation)
                        .Include(x => x.IdBtdNavigation.IdTtNavigation)
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
                return PartialView("_BaiVietDaThichPartial", pagedList);
            }
            return View(pagedList);
        }
        //Bài viết chưa thích
        [Route("ThichBaiViet")]
        [HttpGet]
        public IActionResult ThichBaiViet(int idTK, int? page, string searchTerm)
        {
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            int pageSize = 8;
            // Lấy danh sách bài tuyển dụng chưa ứng tuyển bởi idSyll
            var query = db.Baituyendungs
                                    .Where(b => !db.Thichcongviecs.Any(c => c.IdTk == idTK && c.IdBtd == b.IdBtd))
                                    .Include(b => b.IdLcvNavigation)
                                    .Include(b => b.IdLuongNavigation)
                                    .Include(b =>b.IdTtNavigation)
                                    .AsNoTracking()
                                    .OrderBy(b => b.IdBtd);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.searchTerm = searchTerm;
                query = (IOrderedQueryable<Baituyendung>)query.Where(b =>
                    !db.Thichcongviecs.Any(c => c.IdTk == idTK && c.IdBtd == b.IdBtd) &&
                    (b.Tencongviec.Contains(searchTerm) ||
                    b.IdLcvNavigation.Tenlcv.Contains(searchTerm) ||
                    b.Diachi.Contains(searchTerm) ||
                    b.IdTtNavigation.Tentt.Contains(searchTerm) ||
                    b.IdLuongNavigation.Tenmucluong.Contains(searchTerm)));
            }

            var pagedList = query.ToPagedList(pageNumber, pageSize);

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
                return PartialView("_ThichBaiVietPartial", pagedList);
            }

            return View(pagedList);
        }

        [Route("ThichBaiViet")]
        [HttpPost]
        public IActionResult ThichBaiViet(int idBTD, int IdTK)
        {
            var newRecord = new Thichcongviec { IdBtd = idBTD, IdTk = IdTK };
            db.Thichcongviecs.Add(newRecord);
            db.SaveChanges();
            return RedirectToAction("ThichBaiViet", new { idTK = IdTK });
        }

        [Route("XoaBTDDaThich")]
        [HttpGet]
        public IActionResult XoaBTDDaThich(int idTCV, int idTK)
        {
            TempData["Message"] = "";
            var tcv = db.Thichcongviecs.Find(idTCV);
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
            return RedirectToAction("BaiVietDaThich", "TKNguoiTimViec", new { idTK = idTK });
        }

        [HttpGet]
        [Route("DSBTDUngTuyen")]
        public IActionResult DSBTDUngTuyen(int idTK, int? page, string searchTerm)
        {
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            int pageSize = 8;
            var query = db.CongviecSylls.Where(x => x.IdSyllNavigation.IdTk == idTK)
                        .Include(x => x.IdBtdNavigation)
                        .Include(x => x.IdBtdNavigation.IdLcvNavigation)
                        .Include(x => x.IdBtdNavigation.IdLuongNavigation)
                        .Include(x => x.IdBtdNavigation.IdTtNavigation)
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
                return PartialView("_BTDUngTuyenPartial", pagedList);
            }
            return View(pagedList);
        }

        [Route("XoaBTDUngTuyen")]
        [HttpGet]
        public IActionResult XoaBTDUngTuyen(int idCVSYLL, int idTK)
        {
            TempData["Message"] = "";
            var cvSyll = db.CongviecSylls.Find(idCVSYLL);
            if (cvSyll != null)
            {
                db.CongviecSylls.Remove(cvSyll);
                db.SaveChanges();
                TempData["Message"] = $"Xoá bài viết đã ứng tuyển thành công";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = $"Không tìm thấy";
                TempData["MessageType"] = "error";
            }
            return RedirectToAction("DSBTDUngTuyen", "TKNguoiTimViec", new { idTK = idTK });
        }

        [Route("BTDChuaUT")]
        [HttpGet]
        public IActionResult BTDChuaUT(int idTK, int? page, string searchTerm)
        {
            int pageNumber = page ?? 1;
            int pageSize = 8;

            // Lấy danh sách IdSyll liên kết với idTK
            var idSylls = db.Soyeulyliches.Where(s => s.IdTk == idTK).Select(s => s.IdSyll).ToList();

            var query = db.Baituyendungs
            .Where(b => !db.CongviecSylls.Any(c => idSylls.Contains((int)c.IdSyll) && c.IdBtd == b.IdBtd && c.IdSyllNavigation.IdTk == idTK)) // Thêm điều kiện mới ở đây
            .Include(b => b.IdLcvNavigation)
            .Include(b => b.IdNnNavigation)
            .Include(b => b.IdLuongNavigation)
            .Include(b => b.IdTtNavigation)
            .AsNoTracking()
            .OrderBy(x => x.IdBtd);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.searchTerm = searchTerm;
                query = (IOrderedQueryable<Baituyendung>)query.Where(b =>
                    !db.CongviecSylls.Any(c => idSylls.Contains((int)c.IdSyll) && c.IdBtd == b.IdBtd && c.IdSyllNavigation.IdTk == idTK) &&
                    (b.Tencongviec.Contains(searchTerm) ||
                    b.IdLcvNavigation.Tenlcv.Contains(searchTerm) ||
                    b.Diachi.Contains(searchTerm) ||
                    b.IdTtNavigation.Tentt.Contains(searchTerm) ||
                    b.IdLuongNavigation.Tenmucluong.Contains(searchTerm)));
            }
            ViewBag.ListCV = db.Soyeulyliches.Where(x => x.IdTk == idTK).ToList();

            // Truyền danh sách CV vào ViewBag để sử dụng trong view
            var pagedList = query.ToPagedList(pageNumber, pageSize);

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
                return PartialView("_BTDChuaUTPartial", pagedList);
            }

            return View(pagedList);
        }

        [Route("UVUngTuyen")]
        [HttpGet]
        public IActionResult UVUngTuyen(int idTK, int idBTD)
        {
            // Lấy danh sách CV của tài khoản hiện tại (idTK)
            var listCV = db.Soyeulyliches.Where(x => x.IdTk == idTK).ToList();

            // Truyền danh sách CV vào ViewBag để sử dụng trong view
            ViewBag.ListCV = listCV;

            // Truyền idBTD vào ViewBag để sử dụng trong view
            ViewBag.IdBTD = idBTD;

            return View();
        }

        [HttpPost]
        [Route("UVUngTuyen")]
        public IActionResult UVUngTuyen(int IdTK, int idBTD, int idCV)
        {
            // Kiểm tra xem idTK, idBTD, idCV có tồn tại và hợp lệ không
            var tkExists = db.Taikhoans.Any(t => t.IdTk == IdTK);
            var btdExists = db.Baituyendungs.Any(b => b.IdBtd == idBTD);
            var cvExists = db.Soyeulyliches.Any(cv => cv.IdSyll == idCV && cv.IdTk == IdTK);

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

            return RedirectToAction("BTDChuaUT", "TKNguoiTimViec", new { idTK = IdTK});
        }

    }
}

//[Route("UVUngTuyen")]
//[HttpPost]
//public IActionResult UVUngTuyen(int idBTD, int idSyll)
//{

//}
//var newRecord = new CongviecSyll { IdBtd = idBTD, IdSyll = idSyll };
//db.CongviecSylls.Add(newRecord);
//db.SaveChanges();
//return RedirectToAction("UVUngTuyen", new { idCV = idSyll });
