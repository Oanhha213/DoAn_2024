using DoAnNet7_2.Models;
using DoAnNet7_2.Models.Authentication;
using DoAnNet7_2.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Mvc.Core;
using X.PagedList.Mvc.Core.Common;

namespace DoAnNet7_2.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("TaiKhoan")]
    public class DanhSachTaiKhoanController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();

        [Authentication]
        [HttpGet]
        [Route("")]
        [Route("DanhSachTaiKhoan")]
        public IActionResult DanhSachTaiKhoan(int? page, string searchTerm)
        {
            int pageNumber = page ?? 1;
            int pageSize = 8;

            var query = from tk in db.Taikhoans
                        join anh in db.Anhdaidiens on tk.IdTk equals anh.IdTk into gj
                        from anh in gj.DefaultIfEmpty()
                        join ltk in db.Loaitaikhoans on tk.IdLtk equals ltk.IdLtk into gj2
                        from ltk in gj2.DefaultIfEmpty() /*where ltk.IdLtk == 3*/
                        select new Models.ViewModels.TaiKhoanAnhViewModel
                        {
                            IdTk = tk.IdTk,
                            IdAnhdd = anh != null ? anh.IdAnhdd : 0,
                            Tenanh = anh != null ? anh.Tenanh : null,
                            Hoten = tk.Hoten,
                            Email = tk.Email,
                            Matkhau = tk.Matkhau,
                            Sdt = tk.Sdt,
                            IdLtk = tk.IdLtk,
                            IdLtkNavigation = ltk,
                            CreateAt = tk.CreateAt
                        };

            // Kiểm tra xem có giá trị searchTerm không rồi gán giá trị cho ViewBag.SearchTerm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.searchTerm = searchTerm;
                query = query.Where(tk => tk.Hoten.Contains(searchTerm) || tk.Email.Contains(searchTerm) || tk.Sdt.Contains(searchTerm));
            }
            else
            {
                ViewBag.searchTerm = null; // Đặt ViewBag.SearchTerm về null nếu không có searchTerm
            }

            var pagedList = query.OrderBy(x => x.Hoten).ToPagedList(pageNumber, pageSize);

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
                return PartialView("_TaiKhoanPartial", pagedList);
            }

            return View(pagedList);
        }

        [Authentication]
        [Route("ThemTaiKhoan")]
        [HttpGet]
        public IActionResult ThemTaiKhoan()
        {
            ViewBag.IdLtk = new SelectList(db.Loaitaikhoans.ToList(), "IdLtk","Tenltk");
            return View();
        }

        [Authentication]
        [Route("ThemTaiKhoan")]
        [HttpPost]
        public IActionResult ThemTaiKhoan(Taikhoan tk)
        {
            if (ModelState.IsValid)
            {
                // Set thời gian tạo bài viết
                tk.CreateAt = DateTime.Now;
                db.Taikhoans.Add(tk);
                db.SaveChanges();
                var Id_TK = tk.IdTk;
                HttpContext.Session.SetInt32("IdTk", Id_TK);

                // Chuyển hướng đến action UpAnhDaiDien và truyền vai trò của người dùng
                return RedirectToAction("ThemAnhDaiDien", "Anh");
            }

            return View(tk);
        }

        [Authentication]
        [Route("SuaTaiKhoan")]
        [HttpGet]
        public IActionResult SuaTaiKhoan(int idTK)
        {
            TempData["Message"] = "";
            var tk = db.Taikhoans.FirstOrDefault(x => x.IdTk == idTK);
            if (tk == null)
            {
                NotFound();
            }
            return View(tk);
        }

        [Authentication]
        [Route("SuaTaiKhoan")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaTaiKhoan(Taikhoan tk)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tk).State = EntityState.Modified;
                db.SaveChanges();
                // Truyền thông điệp thành công trực tiếp tới view
                ViewBag.MessageType = "success";
                ViewBag.Message = "Sửa thành công";
                //return View(tk);
                //return RedirectToAction("DanhSachTaiKhoan", "DanhSachTaiKhoan");
                //return RedirectToAction("SuaTaikhoan", "DanhSachTaiKhoan");
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

        [Authentication]
        [Route("XoaTK")]
        [HttpGet]
        public IActionResult XoaTK(int idTK)
        {
            TempData["Message"] = "";
            var tk = db.Taikhoans.Find(idTK);
            var btd = db.Baituyendungs.Where(x => x.IdTk == idTK).ToList();
            var cvsyll = db.CongviecSylls.Where(x => x.IdSyllNavigation.IdTk == idTK).ToList();
            var tcv = db.Thichcongviecs.Where(x => x.IdTk == idTK);
            var anhdd = db.Anhdaidiens.Where(x => x.IdTk == idTK); 
            var ct = db.Congties.Where(x => x.IdTk == idTK);
            var syll = db.Soyeulyliches.Where(x => x.IdTk == idTK);
            if (btd.Count() > 0)
            {
                TempData["Message"] = $"Không xoá được tài khoản này vì tồn tại bài tuyển dụng";
                TempData["MessageType"] = "error";
                return RedirectToAction("DSNhaTuyenDung", "TKNhaTuyenDung");
            }
            if (cvsyll.Count() > 0)
            {
                TempData["Message"] = $"Không xoá được tài khoản này vì ứng viên đã ứng tuyển";
                TempData["MessageType"] = "error";
                return RedirectToAction("DSNguoiTimViec", "TKNguoiTimViec");
            }
            if (tk != null)
            {
                db.Soyeulyliches.RemoveRange(syll);
                db.Thichcongviecs.RemoveRange(tcv);
                db.Congties.RemoveRange(ct);
                db.Anhdaidiens.RemoveRange(anhdd);
                db.Taikhoans.Remove(tk);
                db.SaveChanges();
                TempData["Message"] = $"Xoá tài khoản thành công";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = $"Không tìm thấy tài khoản có ID {tk}";
                TempData["MessageType"] = "error";
            }
            if(tk != null && tk.IdLtk == 2) {
                return RedirectToAction("DSNhaTuyenDung", "TKNhaTuyenDung");
            
            }
            else {
                return RedirectToAction("DSNguoiTimViec", "TKNguoiTimViec");
            }
        }

        [Authentication]
        [Route("InfoTK")]
        [HttpGet]
        public IActionResult InfoTK(int idTK)
        {
            var tk = db.Taikhoans
                .Include(x => x.IdLtkNavigation)
                .FirstOrDefault(x => x.IdTk == idTK);
            ViewBag.Anh = db.Anhdaidiens.FirstOrDefault(x => x.IdTk == idTK);
            var CT = db.Congties.Include(x => x.IdNnNavigation).Include(x => x.IdTtNavigation).FirstOrDefault(x => x.IdTk == idTK);
            var btd = db.Baituyendungs.Where(x => x.IdTk == idTK)
                                        .Include(x => x.IdNnNavigation)
                                        .Include(x => x.IdTthtNavigation)
                                        .Include(x => x.IdTtNavigation)
                                        .Include(x => x.IdLcvNavigation)
                                        .Include(x => x.IdLuongNavigation)
                                        .ToList();
            if (tk != null)
            {
                if(CT != null)
                {
                    ViewBag.ct = CT;
                }
                ViewBag.btd = btd;
                return View(tk);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
