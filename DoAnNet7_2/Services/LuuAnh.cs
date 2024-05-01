using DoAnNet7_2.Models;
using System.Diagnostics;

namespace DoAnNet7_2.Services
{
    public class LuuAnh
    {
        public static string LuuAnhDaiDien(IFormFile anh)
        {
            try
            {
                var tenFile = Guid.NewGuid().ToString() + Path.GetExtension(anh.FileName);
                var duongDan = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Anh", tenFile);

                using (var stream = new FileStream(duongDan, FileMode.Create))
                {
                    anh.CopyTo(stream);
                }

                return "/uploads/Anh/" + tenFile;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi khi lưu file: {ex.Message}");
                return null; // Bạn có thể xử lý lỗi tùy theo ứng dụng của bạn.
            }
        }
    }
}
