using DoAnNet7_2.Models;
using System.Diagnostics;

namespace DoAnNet7_2.Services
{
    public class TTCongViec
    {
        public int IdBtd { get; set; } // Id của bài tuyển dụng
        public string? TTUT { get; set; } // Trạng thái ứng tuyển
        public string? TTThich { get; set; } // Trạng thái thích
        public string? TTHH { get; set; } // Trạng thái hết hạn
    }
}
