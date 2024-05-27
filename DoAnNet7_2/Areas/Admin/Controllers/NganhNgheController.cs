using DoAnNet7_2.Models;
using DoAnNet7_2.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DoAnNet7_2.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("NganhNghe")]
    public class NganhNgheController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();

        [Authentication]
        [Route("DSNganhNghe")]
        [HttpGet]
        public IActionResult DSNganhNghe(int? page, string searchTerm)
        {
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            int pageSize = 8;
            var query = db.Nganhnghes.AsNoTracking()
                        .OrderBy(x => x.Tennganhnghe == "Khác") // Sắp xếp các bản ghi "Khác" xuống cuối danh sách
                        .ThenBy(x => x.Tennganhnghe); // Sắp xếp các bản ghi còn lại theo tên ngành nghề
                                                      // Kiểm tra xem có giá trị searchTerm không rồi gán giá trị cho ViewBag.SearchTerm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.searchTerm = searchTerm;
                query = (IOrderedQueryable<Nganhnghe>)query.Where(x => x.Tennganhnghe.Contains(searchTerm));
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
                return PartialView("_DSNNPartial", pagedList);
            }

            return View(pagedList);
        }

        [Authentication]
        [Route("ThemNganhNghe")]
        [HttpGet]
        public IActionResult ThemNganhNghe()
        {
            return View();
        }

        [Authentication]
        [Route("ThemNganhNghe")]
        [HttpPost]
        public IActionResult ThemNganhNghe(Nganhnghe nn)
        {
            if (ModelState.IsValid)
            {
                // Set thời gian tạo bài viết
                db.Nganhnghes.Add(nn);
                db.SaveChanges();
                ViewBag.ThemNNTC = true;
                //return RedirectToAction("DSNganhNghe", "NganhNghe");
            }
            return View(nn);
        }

        [Authentication]
        [Route("SuaNganhNghe")]
        [HttpGet]
        public IActionResult SuaNganhNghe(int idNN)
        {
            var nn = db.Nganhnghes.FirstOrDefault(x => x.IdNn == idNN);
            return View(nn);
        }

        [Authentication]
        [Route("SuaNganhNghe")]
        [HttpPost]

        public IActionResult SuaNganhNghe(Nganhnghe nn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nn).State = EntityState.Modified;
                db.SaveChanges();
                // Truyền thông điệp thành công trực tiếp tới view
                ViewBag.MessageType = "success";
                ViewBag.Message = "Sửa thành công";
                //return RedirectToAction("DSNganhNghe", "NganhNghe");
            }
            else
            {
                // Truyền thông điệp thất bại trực tiếp tới view
                ViewBag.MessageType = "error";
                ViewBag.Message = "Vui lòng kiểm tra lại thông tin";
            }
            return View(nn);
        }

        [Authentication]
        [Route("XoaNganhNghe")]
        [HttpGet]

        public IActionResult XoaNganhNghe(int idNN)
        {
            TempData["Message"] = "";
            var nn = db.Nganhnghes.Find(idNN);
            var btd = db.Baituyendungs.Where(x => x.IdNn == idNN).ToList();
            var ct = db.Congties.Where(x => x.IdNn == idNN).ToList();
            if (btd.Count() > 0)
            {
                TempData["Message"] = $"Không xoá được ngành nghề vì đã tồn tại trong bài tuyển dụng";
                TempData["MessageType"] = "error";
                return RedirectToAction("DSNganhNghe", "NganhNghe");
            }
            if (ct.Count() > 0)
            {
                TempData["Message"] = $"Không xoá được ngành nghề vì đã tồn tại trong công ty";
                TempData["MessageType"] = "error";
                return RedirectToAction("DSNganhNghe", "NganhNghe");
            }
            if (nn != null)
            {
                db.Nganhnghes.Remove(nn);
                db.SaveChanges();
                TempData["Message"] = $"Xoá ngành nghề thành công";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = $"Không tìm thấy ngành nghề có ID {nn}";
                TempData["MessageType"] = "error";
            }
            return RedirectToAction("DSNganhNghe", "NganhNghe");
        }
    }
}
