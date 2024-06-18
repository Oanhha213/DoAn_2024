using Azure;
using DoAnNet7_2.Models;
using DoAnNet7_2.Models.Authentication;
using DoAnNet7_2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO.Compression;
using X.PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DoAnNet7_2.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("BaiTuyenDung")]
    public class BaiTuyenDungController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();

        [Authentication]
        [HttpGet]
        [Route("DSBaiTuyenDung")]
        public IActionResult DSBaiTuyenDung(int? page, string searchTerm)
        {
            int pageNumber = page ?? 1;
            int pageSize = 8;

            var query = db.Baituyendungs
                        .Include(x => x.IdTkNavigation)
                        .Include(x => x.IdNnNavigation)
                        .Include(x => x.IdLcvNavigation)
                        .Include(x => x.IdLuongNavigation)
                        .Include(x => x.IdTthtNavigation)
                        .AsNoTracking()
                        .OrderByDescending(x => x.IdBtd);

            // Kiểm tra xem có giá trị searchTerm không rồi gán giá trị cho ViewBag.SearchTerm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.searchTerm = searchTerm;
                query = (IOrderedQueryable<Baituyendung>)query
                    .Where(x => x.Tencongviec.Contains(searchTerm) || x.IdNnNavigation.Tennganhnghe.Contains(searchTerm)
                    || x.IdTkNavigation.Email.Contains(searchTerm)
                    );
            }
            else
            {
                ViewBag.searchTerm = null; // Đặt ViewBag.SearchTerm về null nếu không có searchTerm
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
                return PartialView("_DSBTDPartial", pagedList);
            }
            
            return View(pagedList);
        }

        [Authentication]
        [HttpGet]
        public async Task<IActionResult> DoiTTHT(int idBTD, string newStatus, string currentView)
        {
            // Tìm bài tuyển dụng trong cơ sở dữ liệu
            var btd = await db.Baituyendungs.FindAsync(idBTD);
            if (btd == null)
            {
                return NotFound();
            } 
            int IdTK = (int)btd.IdTk;
            Debug.WriteLine("IdTK:" + IdTK);
            ViewBag.idTK = IdTK;

            // Cập nhật trạng thái hiển thị mới
            btd.IdTtht = await db.Trangthaihienthis
                                    .Where(t => t.Tenttht == newStatus)
                                    .Select(t => t.IdTtht)
                                    .FirstOrDefaultAsync();

            try
            {
                await db.SaveChangesAsync();
                // Chuyển về trang danh sách bài tuyển dụng hoặc trang công ty tùy thuộc vào currentView
                if (currentView == "DSBaiTuyenDung")
                {
                    return RedirectToAction("DSBaiTuyenDung","BaiTuyenDung");
                }
                else if (currentView == "DSBTDCongTy")
                {
                    // Cần truyền lại tham số idTK nếu chuyển về trang DSBTDCongTy
                    return RedirectToAction("DSBTDCongTy", "TKNhaTuyenDung", new { idTK = IdTK });
                }
            }
            catch (DbUpdateException)
            {
                // Xử lý lỗi nếu cần
                return RedirectToAction(currentView, "BaiTuyenDung");
            }
            return RedirectToAction("DSBaiTuyenDung", "BaiTuyenDung");
        }

        [Authentication]
        [Route("ChiTietBTD")]
        public IActionResult ChiTietBTD(int idBTD)
        {
            var ID_SYLL = HttpContext.Session.GetInt32("IdSyll");
            var btd = db.Baituyendungs.FirstOrDefault(x => x.IdBtd == idBTD);
            if (btd == null)
            {
                return RedirectToAction("TrangLoi", "Home"); ; 
            }
            ViewBag.Tencongviec = btd.Tencongviec;
            db.Entry(btd).Reference(x => x.IdTgknNavigation).Load();
            db.Entry(btd).Reference(x => x.IdLcvNavigation).Load();
            db.Entry(btd).Reference(x => x.IdNnNavigation).Load();
            db.Entry(btd).Reference(x => x.IdTkNavigation).Load();
            db.Entry(btd).Reference(x => x.IdTtNavigation).Load();
            db.Entry(btd).Reference(x => x.IdLuongNavigation).Load();
            db.Entry(btd).Reference(x => x.IdTthtNavigation).Load();

            return View(btd);
        }

        [Authentication]
        [Route("DSUngTuyen")]
        [HttpGet]
        public IActionResult DSUngTuyen(int idBTD, int? page, string searchTerm)
        {
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            int pageSize = 8;
            var query = db.CongviecSylls.Where(x => x.IdBtd == idBTD)
                        .Include(x=>x.IdSyllNavigation.IdTkNavigation)
                        .AsNoTracking()
                        .OrderBy(x => x.IdCvsyll);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.searchTerm = searchTerm;
                query = (IOrderedQueryable<CongviecSyll>)query
                    .Where(x => x.IdSyllNavigation.IdTkNavigation.Hoten.Contains(searchTerm) ||
                            x.IdSyllNavigation.IdTkNavigation.Email.Contains(searchTerm) ||
                            x.IdSyllNavigation.IdTkNavigation.Sdt.Contains(searchTerm));
            }
            else
            {
                ViewBag.searchTerm = null; // Đặt ViewBag.SearchTerm về null nếu không có searchTerm
            }

            var pagedList = query.OrderBy(x => x.IdCvsyll).ToPagedList(pageNumber, pageSize);
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
                return PartialView("_DSUTPartial", pagedList);
            }
            ViewBag.Tencongviec = db.Baituyendungs.FirstOrDefault(x => x.IdBtd == idBTD)?.Tencongviec;
            ViewBag.idBTD = idBTD;
            return View(pagedList);
        }

        [Authentication]
        [Route("XoaUT")]
        [HttpGet]
        public IActionResult XoaUT(int idCVSYLL, int idBTD)
        {
            TempData["Message"] = "";
            var cvsyll = db.CongviecSylls.Find(idCVSYLL);
            if (cvsyll != null)
            {
                db.CongviecSylls.Remove(cvsyll);
                db.SaveChanges(); // Đẩy thay đổi vào database ngay sau khi xoá bài tuyển dụng
                TempData["Message"] = $"Xoá ứng viên ứng tuyển thành công";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = $"Không tìm thấy";
                TempData["MessageType"] = "error";
            }
            return RedirectToAction("DSUngTuyen", "BaiTuyenDung", new {idBTD = idBTD});
        }

        [Authentication]
        [Route("BTDUngVien")]
        [HttpGet]
        public IActionResult BTDUngVien( int? page, string searchTerm)
        {
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            int pageSize = 8;
            var query = db.CongviecSylls.Include(x => x.IdSyllNavigation)
                        .Include(x => x.IdSyllNavigation.IdTkNavigation)
                        .Include(x => x.IdBtdNavigation)
                        .AsNoTracking()
                        .OrderBy(x => x.IdSyllNavigation.IdTk);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.searchTerm = searchTerm;
                query = (IOrderedQueryable<CongviecSyll>)query
                    .Where(x => x.IdSyllNavigation.IdTkNavigation.Hoten.Contains(searchTerm) ||
                            x.IdSyllNavigation.IdTkNavigation.Email.Contains(searchTerm) ||
                            x.IdSyllNavigation.IdTkNavigation.Sdt.Contains(searchTerm) ||
                            x.IdSyllNavigation.Tensyll.Contains(searchTerm) ||
                            x.IdBtdNavigation.Tencongviec.Contains(searchTerm));
            }
            else
            {
                ViewBag.searchTerm = null; // Đặt ViewBag.SearchTerm về null nếu không có searchTerm
            }

            var pagedList = query.OrderBy(x => x.IdSyllNavigation.IdTk).ToPagedList(pageNumber, pageSize);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_BTDUngVienPartial", pagedList);
            }
            return View(pagedList);
        }

        //[Route("UngTuyen")]
        //[HttpGet]
        //public IActionResult UngTuyen()
        //{
        //    ViewBag.BaiTuyenDung = db.Baituyendungs.ToList();
        //    ViewBag.SoYeuLyLich = db.Soyeulyliches.Include(x => x.IdTkNavigation).ToList();
        //    return View();
        //}

        //[Route("UngTuyen")]
        //[HttpPost]
        //public IActionResult UngTuyen(CongviecSyll ut)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.CongviecSylls.Add(ut);
        //        db.SaveChanges();

        //        // Chuyển hướng đến action UpAnhDaiDien và truyền vai trò của người dùng
        //        return RedirectToAction("BTDUngVien", "BaiTuyenDung");
        //    }
        //    ViewBag.BaiTuyenDung = db.Baituyendungs.ToList();
        //    ViewBag.SoYeuLyLich = db.Soyeulyliches.Include(x=>x.IdTkNavigation).ToList();
        //    return View(ut);
        //}

        [Authentication]
        [Route("ThemBTDCongTy")]
        [HttpGet]
        public IActionResult ThemBTDCongTy()
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");
            ViewBag.IdTk = ID_TK;
            ViewBag.IdNn = new SelectList(db.Nganhnghes.ToList(), "IdNn", "Tennganhnghe");
            ViewBag.IdLcv = new SelectList(db.Loaicongviecs.ToList(), "IdLcv", "Tenlcv");
            ViewBag.IdTgkn = new SelectList(db.Thoigiankinhnghiems.ToList(), "IdTgkn", "Tentgkn");
            ViewBag.IdLuong = new SelectList(db.Luongs.ToList(), "IdLuong", "Tenmucluong");
            ViewBag.IdTt = new SelectList(db.Tinhthanhs.ToList(), "IdTt", "Tentt");
            ViewBag.IdTtht = new SelectList(db.Trangthaihienthis.ToList(), "IdTtht", "Tenttht");
            return View();
        }

        [Authentication]
        [Route("ThemBTDCongTy")]
        [HttpPost]

        public IActionResult ThemBTDCongTy(Baituyendung btd)
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");
            ViewBag.IdTk = ID_TK;
                if (ModelState.IsValid)
                {
                    // Set thời gian tạo bài viết
                    btd.CreateAt = DateTime.Now;
                    btd.IdTk = ID_TK;
                    db.Baituyendungs.Add(btd);
                    db.SaveChanges();
                    return RedirectToAction("DSBTDCongTy", "TKNhaTuyenDung", new { idTK = ID_TK });
                }
                else
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Debug.WriteLine(error.ErrorMessage);
                    }
                }
            ViewBag.IdNn = new SelectList(db.Nganhnghes.ToList(), "IdNn", "Tennganhnghe");
            ViewBag.IdLcv = new SelectList(db.Loaicongviecs.ToList(), "IdLcv", "Tenlcv");
            ViewBag.IdTgkn = new SelectList(db.Thoigiankinhnghiems.ToList(), "IdTgkn", "Tentgkn");
            ViewBag.IdLuong = new SelectList(db.Luongs.ToList(), "IdLuong", "Tenmucluong");
            ViewBag.IdTt = new SelectList(db.Tinhthanhs.ToList(), "IdTt", "Tentt");
            ViewBag.IdTtht = new SelectList(db.Trangthaihienthis.ToList(), "IdTt", "Tenttht");
            return View(btd);
        }

        [Authentication]
        [Route("ThemBTD")]
        [HttpGet]
        public IActionResult ThemBTD()
        {
            ViewBag.IdTk = new SelectList(db.Taikhoans.Where(x => x.IdLtk == 2).ToList(), "IdTk", "Email");
            ViewBag.IdNn = new SelectList(db.Nganhnghes.ToList(), "IdNn", "Tennganhnghe");
            ViewBag.IdLcv = new SelectList(db.Loaicongviecs.ToList(), "IdLcv", "Tenlcv");
            ViewBag.IdTgkn = new SelectList(db.Thoigiankinhnghiems.ToList(), "IdTgkn", "Tentgkn");
            ViewBag.IdTglv = new SelectList(db.Thoigianlamviecs.ToList(), "IdTglv", "Tentglv");
            ViewBag.IdLuong = new SelectList(db.Luongs.ToList(), "IdLuong", "Tenmucluong");
            ViewBag.IdTt = new SelectList(db.Tinhthanhs.ToList(), "IdTt", "Tentt");
            ViewBag.IdTtht = new SelectList(db.Trangthaihienthis.ToList(), "IdTtht", "Tenttht");
            ViewBag.Taikhoan = db.Taikhoans.Where(x => x.IdLtk == 2).ToList();
            return View();
        }

        [Authentication]
        [Route("ThemBTD")]
        [HttpPost]

        public IActionResult ThemBTD(Baituyendung btd)
        {
            if (ModelState.IsValid)
            {
                // Set thời gian tạo bài viết
                btd.CreateAt = DateTime.Now;
                db.Baituyendungs.Add(btd);
                db.SaveChanges();
                ViewBag.ThemBTDTC = true;
                //return RedirectToAction("DSBaiTuyenDung", "BaiTuyenDung");
            }
            ViewBag.Taikhoan = db.Taikhoans.Where(x => x.IdLtk == 2).ToList();
            return View(btd);
        }

        [Authentication]
        [Route("SuaBTD")]
        [HttpGet]
        public IActionResult SuaBTD(int idBTD)
        {
            var btd = db.Baituyendungs.FirstOrDefault(x => x.IdBtd == idBTD);
            ViewBag.IdNn = new SelectList(db.Nganhnghes.ToList(), "IdNn", "Tennganhnghe");
            ViewBag.IdLcv = new SelectList(db.Loaicongviecs.ToList(), "IdLcv", "Tenlcv");
            ViewBag.IdTgkn = new SelectList(db.Thoigiankinhnghiems.ToList(), "IdTgkn", "Tentgkn");
            ViewBag.IdTglv = new SelectList(db.Thoigianlamviecs.ToList(), "IdTglv", "Tentglv");
            ViewBag.IdLuong = new SelectList(db.Luongs.ToList(), "IdLuong", "Tenmucluong");
            ViewBag.IdTt = new SelectList(db.Tinhthanhs.ToList(), "IdTt", "Tentt");
            ViewBag.IdTtht = new SelectList(db.Trangthaihienthis.ToList(), "IdTtht", "Tenttht");
            return View(btd);
        }

        [Authentication]
        [Route("SuaBTD")]
        [HttpPost]

        public IActionResult SuaBTD(Baituyendung btd)
        {
            if (ModelState.IsValid)
            {
                db.Entry(btd).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.MessageType = "success";
                ViewBag.Message = "Sửa thành công";
                //return RedirectToAction("DSBaiTuyenDung", "BaiTuyenDung");
            }
            else
            {
                // Truyền thông điệp thất bại trực tiếp tới view
                ViewBag.MessageType = "error";
                ViewBag.Message = "Vui lòng kiểm tra lại thông tin";
            }
            return View(btd);
        }

        [Authentication]
        [Route("XoaBTD")]
        [HttpGet]
        public IActionResult XoaBTD(int idBTD)
        {
            TempData["Message"] = "";
            var cvsyll = db.CongviecSylls.Where(x => x.IdBtd == idBTD);
            var btd = db.Baituyendungs.Find(idBTD);
            if (btd != null)
            {
                db.CongviecSylls.RemoveRange(cvsyll);
                db.Baituyendungs.Remove(btd);
                db.SaveChanges(); // Đẩy thay đổi vào database ngay sau khi xoá bài tuyển dụng
                TempData["Message"] = $"Xoá bài tuyển dụng thành công";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = $"Không tìm thấy bài tuyển dụng có ID {idBTD}";
                TempData["MessageType"] = "error";
            }
            return RedirectToAction("DSBaiTuyenDung","BaiTuyenDung");
        }

        [Authentication]
        [Route("XoaBTDUngVien")]
        [HttpGet]
        public IActionResult XoaBTDUngVien(int idBTDUV)
        {
            TempData["Message"] = "";
            var cvsyll = db.CongviecSylls.Find(idBTDUV);
            if (cvsyll != null)
            {
                db.CongviecSylls.Remove(cvsyll);
                db.SaveChanges(); // Đẩy thay đổi vào database ngay sau khi xoá bài tuyển dụng
                TempData["Message"] = $"Xoá bài tuyển dụng - ứng viên thành công";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = $"Không tìm thấy bài tuyển dụng - ứng viên có ID {cvsyll}";
                TempData["MessageType"] = "error";
            }
            return RedirectToAction("BTDUngVien", "BaiTuyenDung");
        }
    }
}
