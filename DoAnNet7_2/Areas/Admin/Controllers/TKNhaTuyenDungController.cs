using DoAnNet7_2.Models;
using DoAnNet7_2.Models.ViewModels;
using DoAnNet7_2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DoAnNet7_2.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("TaiKhoan")]
    public class TKNhaTuyenDungController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();

        [Route("")]
        [Route("DSNhaTuyenDung")]
        [HttpGet]
        public IActionResult DSNhaTuyenDung(int? page, string searchTerm)
        {
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            int pageSize = 8;
            var query = from tk in db.Taikhoans
                        join ct in db.Congties on tk.IdTk equals ct.IdTk into gj
                        from ct in gj.DefaultIfEmpty()
                        where tk.IdLtk == 2
                        select new Models.ViewModels.TaiKhoanCongTyViewModel
                        {
                            IdTk = tk.IdTk,
                            IdCt = ct != null ? ct.IdCt : 0,
                            Hoten = tk.Hoten,
                            Email = tk.Email,
                            Matkhau = tk.Matkhau,
                            Sdt = tk.Sdt,
                            Logo = ct != null ? ct.Logo : null,
                            Tencongty = ct != null ? ct.Tencongty : null
                        };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.searchTerm = searchTerm;
                query = query.Where(x => x.Hoten.Contains(searchTerm) 
                || x.Email.Contains(searchTerm) || x.Sdt.Contains(searchTerm) || x.Tencongty.Contains(searchTerm));
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
                return PartialView("_DSNTDPartial", pagedList);
            }
            return View(pagedList);
        }

        [HttpGet]
        [Route("DSBTDCongTy")]
        public IActionResult DSBTDCongTy(int idTK, int? page, string searchTerm)
        {
            int pageNumber = page ?? 1;
            int pageSize = 8;
            var query = db.Baituyendungs.Include(x => x.IdTkNavigation)
                        .Where(tk => tk.IdTk == idTK)
                        .Include(x => x.IdNnNavigation)
                        .Include(x => x.IdLcvNavigation)
                        .Include(x => x.IdLuongNavigation)
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
            // Sắp xếp dữ liệu trước khi phân trang
            query = query.OrderBy(x => x.Tencongviec);

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
                return PartialView("_BTDCongTyPartial", pagedList);
            }
            ViewBag.idTK = idTK;
            var HT = db.Taikhoans.FirstOrDefault(x => x.IdTk == idTK)?.Hoten;
            ViewBag.Ho_Ten = HT;
            var Id_TK = idTK;
            HttpContext.Session.SetInt32("IdTk", Id_TK);
            return View(pagedList);
        }

        [Route("ThemCongTy")]
        [HttpGet]
        public IActionResult ThemCongTy()
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");

            if (!ID_TK.HasValue)
            {
                // Xử lý trường hợp người dùng không hợp lệ, có thể chuyển hướng hoặc xử lý lỗi khác
                return RedirectToAction("TrangLoi", "Home");
            }

            ViewBag.IdNn = new SelectList(db.Nganhnghes.ToList(), "IdNn", "Tennganhnghe");
            ViewBag.IdTt = new SelectList(db.Tinhthanhs.ToList(), "IdTt", "Tentt");
            ViewBag.Email = db.Taikhoans.FirstOrDefault(x => x.IdTk == ID_TK)?.Email;
            var ct = new Congty
            {
                IdTk = ID_TK.Value
            };

            return View(ct);
        }

        [Route("ThemCongTy")]
        [HttpPost]
        public IActionResult ThemCongTy(Congty ct)
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");
            if (!ID_TK.HasValue)
            {
                // Xử lý trường hợp người dùng không hợp lệ, có thể chuyển hướng hoặc xử lý lỗi khác
                return RedirectToAction("Trangloi", "Home");
            }

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
                return RedirectToAction("DSNhaTuyenDung", "TKNhaTuyenDung");
            }
            return View(ct);
        }

        [Route("CongTy")]
        public IActionResult CongTy(int IdTK)
        {
            var ct = db.Congties.FirstOrDefault(x => x.IdTk == IdTK);
            var Id_TK = IdTK;
            HttpContext.Session.SetInt32("IdTk", Id_TK);
            //if (ct == null)
            //{
            //    return RedirectToAction("ThemCongTy", "TKNhaTuyenDung");
            //}
            if(ct != null)
            {
                ViewBag.Tenct = ct.Tencongty;
                // Include các dữ liệu liên quan nếu cần thiết
                db.Entry(ct).Reference(x => x.IdNnNavigation).Load();
                db.Entry(ct).Reference(x => x.IdTtNavigation).Load();
                return View(ct);
            }
            return View(ct);
        }

        [Route("SuaCongTy")]
        [HttpGet]
        public IActionResult SuaCongTy(int idTK)
        {
            var ct = db.Congties.FirstOrDefault(a => a.IdTk == idTK);
            var Id_TK = idTK;
            HttpContext.Session.SetInt32("IdTk", Id_TK);
            if (ct == null)
            {
                return RedirectToAction("ThemCongTy", "TKNhaTuyenDung");
            }
            ViewBag.Tenct = ct.Tencongty;
            ViewBag.IdNn = new SelectList(db.Nganhnghes.ToList(), "IdNn", "Tennganhnghe");
            ViewBag.IdTt = new SelectList(db.Tinhthanhs.ToList(), "IdTt", "Tentt");

            return View(ct);
        }

        [Route("SuaCongTy")]
        [HttpPost]
        public IActionResult SuaCongTy(Congty ct)
        {
            var existingCT = db.Congties.FirstOrDefault(a => a.IdTk == ct.IdTk);
            if (existingCT == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid || (!string.IsNullOrEmpty(ct.Tencongty)))
            {
                // Kiểm tra xem người dùng đã tải lên logo mới hay không
                if (ct.UpLogo != null)
                {
                    // Nếu có, lưu ảnh mới
                    existingCT.Logo = LuuAnh.LuuAnhDaiDien(ct.UpLogo);
                }

                // Cập nhật các trường khác
                existingCT.Tencongty = ct.Tencongty;
                existingCT.IdNn = ct.IdNn;
                existingCT.Sonhanvien = ct.Sonhanvien;
                existingCT.Diachi = ct.Diachi;
                existingCT.IdTt = ct.IdTt;
                existingCT.Mota = ct.Mota;

                // Cập nhật thông tin trong cơ sở dữ liệu
                db.SaveChanges();
                ViewBag.MessageType = "success";
                ViewBag.Message = "Sửa thành công";
                //return RedirectToAction("DSNhaTuyenDung", "TKNhaTuyenDung");
            }
            else
            {
                // Truyền thông điệp thất bại trực tiếp tới view
                ViewBag.MessageType = "error";
                ViewBag.Message = "Vui lòng kiểm tra lại thông tin";
            }
            return View(ct);
        }
        [Route("XoaCongTy")]
        [HttpGet]
        public IActionResult XoaCongTy(int idCT)
        {
            TempData["Message"] = "";
            var ct = db.Congties.Find(idCT);
            if (ct != null)
            {
                db.Congties.Remove(ct);
                db.SaveChanges(); // Đẩy thay đổi vào database ngay sau khi xoá bài tuyển dụng
                TempData["Message"] = $"Xoá công ty thành công";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = $"Không tìm thấy công ty có ID {idCT}";
                TempData["MessageType"] = "error";
            }
            return RedirectToAction("DSNhaTuyenDung", "TKNhaTuyenDung");
        }
    }
}
