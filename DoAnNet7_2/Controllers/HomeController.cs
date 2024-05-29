using DoAnNet7_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

namespace DoAnNet7_2.Controllers
{
    public class HomeController : Controller
    {
        Jobsworld2Context db = new Jobsworld2Context();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(int? page, string searchTerm, string luong, string tinhthanh, string nganhnghe, string kinhnghiem)
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
                        .OrderByDescending(x => x.IdBtd);

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
                return PartialView("_IndexPartial", pagedList);
            }

            return View(pagedList);
        }

        public IActionResult Trangloi()
        {
            return View();
        }

        [HttpGet]
        [Route("DSCongTyHome")]
        public IActionResult DSCongTyHome(int? page,string searchTerm)
        {
            int pageNumber = page ?? 1;
            int pageSize = 8;
            IQueryable<Congty> query = db.Congties; // Sử dụng IQueryable để xây dựng câu truy vấn LINQ
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.searchTerm = searchTerm;
                // Lọc các công ty theo từ khóa tìm kiếm
                query = query.Where(x => x.Tencongty.Contains(searchTerm));
            }
            var ID_TK = HttpContext.Session.GetInt32("IdTk");
            var pagedList = query.ToPagedList(pageNumber, pageSize);
            if (ID_TK.HasValue)
            {
                HttpContext.Session.SetInt32("IdTk", (int)ID_TK);
            }
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DSCongTyHomePartial", pagedList);
            }
            return View(pagedList);
        }

        [HttpGet]
        [Route("VeChungToi")]
        public IActionResult AboutUs()
        {
            return View();
        }

        [HttpGet]
        [Route("LienHe")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpGet]
        [Route("ChiTietCTHome")]
        public IActionResult ChiTietCTHome(int idTK)
        {
            var ID_TK = HttpContext.Session.GetInt32("IdTk");

            var ct = db.Congties.Include(x => x.IdNnNavigation)
                                .Include(x => x.IdTtNavigation)
                                .FirstOrDefault(x => x.IdTk == idTK);
            if (ct == null)
            {
                // Xử lý khi không tìm thấy công ty, có thể chuyển hướng đến trang lỗi hoặc hiển thị thông báo
                return NotFound();
            }
            var btd = db.Baituyendungs.Where(x => x.IdTk == idTK).Include(x => x.IdTtNavigation)
                                        .Include(x => x.IdLuongNavigation)
                                        .Include(x => x.IdLcvNavigation);
            if (ID_TK.HasValue)
            {
                ViewBag.IdTk = ID_TK.Value;
            }
            else
            {
                ViewBag.IdTk = null;
            }
            if(btd.Count() >0)
            {
                ViewBag.btd = btd.ToList();
            }
            else
            {
                ViewBag.btd = null;
            }
            return View(ct);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}