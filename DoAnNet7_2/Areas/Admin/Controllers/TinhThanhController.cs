using DoAnNet7_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace DoAnNet7_2.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("TinhThanh")]
    public class TinhThanhController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();
        [Route("DSTinhThanh")]
        [HttpGet]
        public IActionResult DSTinhThanh(int? page, string searchTerm)
        {
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            int pageSize = 8;
            var query = db.Tinhthanhs.AsNoTracking()
                        .OrderBy(x => x.Tentt == "Khác") // Sắp xếp các bản ghi "Khác" xuống cuối danh sách
                        .ThenBy(x => x.Tentt); // Sắp xếp các bản ghi còn lại theo tên ngành nghề
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.searchTerm = searchTerm;
                query = (IOrderedQueryable<Tinhthanh>)query.Where(x => x.Tentt.Contains(searchTerm));
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
                return PartialView("_DSTTPartial", pagedList);
            }

            return View(pagedList);
        }

        [Route("ThemTinhThanh")]
        [HttpGet]
        public IActionResult ThemTinhThanh()
        {
            return View();
        }

        [Route("ThemTinhThanh")]
        [HttpPost]
        public IActionResult ThemTinhThanh(Tinhthanh tt)
        {
            if (ModelState.IsValid)
            {
                // Set thời gian tạo bài viết
                db.Tinhthanhs.Add(tt);
                db.SaveChanges();
                return RedirectToAction("DSTinhThanh", "TinhThanh");
            }
            return View(tt);
        }

        [Route("SuaTinhThanh")]
        [HttpGet]
        public IActionResult SuaTinhThanh(int idTT)
        {
            var tt = db.Tinhthanhs.FirstOrDefault(x => x.IdTt == idTT);
            return View(tt);
        }

        [Route("SuaTinhThanh")]
        [HttpPost]

        public IActionResult SuaTinhThanh(Tinhthanh tt)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tt).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("DSTinhThanh", "TinhThanh");
                ViewBag.MessageType = "success";
                ViewBag.Message = "Sửa thành công";
            }
            else
            {
                // Truyền thông điệp thất bại trực tiếp tới view
                ViewBag.MessageType = "error";
                ViewBag.Message = "Vui lòng kiểm tra lại thông tin";
            }
            return View(tt);
        }

        [Route("XoaTinhThanh")]
        [HttpGet]

        public IActionResult XoaTinhThanh(int idTT)
        {
            TempData["Message"] = "";
            var tt = db.Tinhthanhs.Find(idTT);
            var btd = db.Baituyendungs.Where(x => x.IdTt == idTT).ToList();
            var ct = db.Congties.Where(x => x.IdTt == idTT).ToList();
            if (btd.Count() > 0)
            {
                TempData["Message"] = $"Không xoá được tỉnh thành vì đã tồn tại trong bài tuyển dụng";
                TempData["MessageType"] = "error";
                return RedirectToAction("DSTinhThanh", "TinhThanh");
            }
            if (ct.Count() > 0)
            {
                TempData["Message"] = $"Không xoá được tỉnh thành vì đã tồn tại trong công ty";
                TempData["MessageType"] = "error";
                return RedirectToAction("DSTinhThanh", "TinhThanh");
            }
            if (tt != null)
            {
                db.Tinhthanhs.Remove(tt);
                db.SaveChanges();
                TempData["Message"] = $"Xoá tỉnh thành thành công";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = $"Không tìm thấy tỉnh thành có ID {tt}";
                TempData["MessageType"] = "error";
            }
            return RedirectToAction("DSTinhThanh", "TinhThanh");
        }
    }
}
