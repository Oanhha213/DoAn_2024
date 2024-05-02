using DoAnNet7_2.Models;
using DoAnNet7_2.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DoAnNet7_2.Services;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using Microsoft.AspNetCore.Http;

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

            // Mục 3: Kiểm tra ModelState Errors
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine(error.ErrorMessage);
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

        [HttpGet]
        public async Task<IActionResult> DoiTTHT(int idBTD, string newStatus)
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
                return RedirectToAction("DSBTDNTD", "NhaTuyenDung");
            }
            catch (DbUpdateException e)
            {
                Debug.WriteLine("lỗi:" + e);
            }
            return RedirectToAction("DSBTDNTD", "NhaTuyenDung");
        }

        [Authentication]
        [HttpGet]
        [Route("DSBTDNTD")]
        public IActionResult DSBTDNTD(int? page, string searchTerm)
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");
            int pageNumber = page ?? 1;
            int pageSize = 8;
            var query = db.Baituyendungs.Include(x => x.IdTkNavigation)
                        .Where(tk => tk.IdTk == ID_TK)
                        .Include(x => x.IdNnNavigation)
                        .Include(x => x.IdLcvNavigation)
                        .Include(x => x.IdTglvNavigation)
                        .Include(x => x.IdTthtNavigation)
                        .AsNoTracking()
                        .OrderBy(x => x.IdBtd);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.SearchTerm = searchTerm;
                query = (IOrderedQueryable<Baituyendung>)query.Where(x => x.Tencongviec.Contains(searchTerm)
                                                                || x.IdNnNavigation.Tennganhnghe.Contains(searchTerm)
                                                                || x.IdLcvNavigation.Tenlcv.Contains(searchTerm)
                                                                );
            }
            else
            {
                ViewBag.SearchTerm = null; // Đặt ViewBag.SearchTerm về null nếu không có searchTerm
            }

            var pagedList = query.ToPagedList(pageNumber, pageSize);
            ViewBag.idTK = ID_TK;
            ViewBag.Ho_Ten = db.Taikhoans.FirstOrDefault(x => x.IdTk == ID_TK)?.Hoten;
            var idTK = ID_TK;
            if (idTK.HasValue)
            {
                HttpContext.Session.SetInt32("IdTk", (int)idTK);
                // Tiếp tục xử lý với giá trị của idTK ở đây
            }
            else
            {
                idTK = HttpContext.Session.GetInt32("IdTk");
            }
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
                return PartialView("_DSBTDNTDPartial", pagedList);
            }
            
            return View(pagedList);
        }

        [Authentication]
        [Route("ChiTietBTDNTD")]
        public IActionResult ChiTietBTDNTD(int idBTD)
        {
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
            db.Entry(btd).Reference(x => x.IdTglvNavigation).Load();
            db.Entry(btd).Reference(x => x.IdTthtNavigation).Load();

            return View(btd);
        }

        [Route("DSUngTuyenNTD")]
        [HttpGet]
        public IActionResult DSUngTuyenNTD(int idBTD, int? page, string searchTerm)
        {
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            int pageSize = 8;
            var query = db.CongviecSylls.Where(x => x.IdBtd == idBTD)
                        .Include(x => x.IdSyllNavigation.IdTkNavigation)
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
                return PartialView("_DSUngTuyenNTDPartial", pagedList);
            }
            ViewBag.Tencongviec = db.Baituyendungs.FirstOrDefault(x => x.IdBtd == idBTD)?.Tencongviec;
            ViewBag.idBTD = idBTD;
            return View(pagedList);
        }

        [Authentication]
        [Route("XoaUTNTD")]
        [HttpGet]
        public IActionResult XoaUTNTD(int idCVSYLL, int idBTD)
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
            return RedirectToAction("DSUngTuyen", "BaiTuyenDung", new { idBTD = idBTD });
        }

        [Authentication]
        [Route("ThemBTDNTD")]
        [HttpGet]
        public IActionResult ThemBTDNTD()
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");
            ViewBag.IdTk = ID_TK;
            ViewBag.IdNn = new SelectList(db.Nganhnghes.ToList(), "IdNn", "Tennganhnghe");
            ViewBag.IdLcv = new SelectList(db.Loaicongviecs.ToList(), "IdLcv", "Tenlcv");
            ViewBag.IdTgkn = new SelectList(db.Thoigiankinhnghiems.ToList(), "IdTgkn", "Tentgkn");
            ViewBag.IdTglv = new SelectList(db.Thoigianlamviecs.ToList(), "IdTglv", "Tentglv");
            ViewBag.IdTt = new SelectList(db.Tinhthanhs.ToList(), "IdTt", "Tentt");
            ViewBag.IdTtht = new SelectList(db.Trangthaihienthis.ToList(), "IdTtht", "Tenttht");
            return View();
        }

        [Authentication]
        [Route("ThemBTDNTD")]
        [HttpPost]

        public IActionResult ThemBTDNTD(Baituyendung btd)
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
                return RedirectToAction("DSBTDNTD", "NhaTuyenDung");
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
            ViewBag.IdTglv = new SelectList(db.Thoigianlamviecs.ToList(), "IdTglv", "Tentglv");
            ViewBag.IdTt = new SelectList(db.Tinhthanhs.ToList(), "IdTt", "Tentt");
            ViewBag.IdTtht = new SelectList(db.Trangthaihienthis.ToList(), "IdTt", "Tenttht");
            return View(btd);
        }

        [Authentication]
        [Route("SuaBTDNTD")]
        [HttpGet]
        public IActionResult SuaBTDNTD(int idBTD)
        {
            var btd = db.Baituyendungs.FirstOrDefault(x => x.IdBtd == idBTD);
            ViewBag.IdNn = new SelectList(db.Nganhnghes.ToList(), "IdNn", "Tennganhnghe");
            ViewBag.IdLcv = new SelectList(db.Loaicongviecs.ToList(), "IdLcv", "Tenlcv");
            ViewBag.IdTgkn = new SelectList(db.Thoigiankinhnghiems.ToList(), "IdTgkn", "Tentgkn");
            ViewBag.IdTglv = new SelectList(db.Thoigianlamviecs.ToList(), "IdTglv", "Tentglv");
            ViewBag.IdTt = new SelectList(db.Tinhthanhs.ToList(), "IdTt", "Tentt");
            ViewBag.IdTtht = new SelectList(db.Trangthaihienthis.ToList(), "IdTtht", "Tenttht");
            return View(btd);
        }

        [Authentication]
        [Route("SuaBTDNTD")]
        [HttpPost]

        public IActionResult SuaBTDNTD(Baituyendung btd)
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
        [Route("XoaBTDNTD")]
        [HttpGet]
        public IActionResult XoaBTDNTD(int idBTD)
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
            return RedirectToAction("DSBTDNTD", "NhaTuyenDung");
        }
    }
}
