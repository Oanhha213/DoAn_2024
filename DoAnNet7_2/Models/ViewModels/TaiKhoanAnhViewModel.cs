using System.ComponentModel;

namespace DoAnNet7_2.Models.ViewModels
{
    public class TaiKhoanAnhViewModel
    {
        [DisplayName("Tài khoản")]
        public int IdTk { get; set; }
        public int IdAnhdd { get; set; }

        [DisplayName("Tên ảnh")]
        public string? Tenanh { get; set; }

        [DisplayName("Họ tên")]
        public string Hoten { get; set; } = null!;

        public string Email { get; set; } = null!;

        [DisplayName("Mật khẩu")]
        public string Matkhau { get; set; } = null!;

        [DisplayName("Số điện thoại")]
        public string? Sdt { get; set; }

        [DisplayName("Loại tài khoản")]
        public int? IdLtk { get; set; }

        [DisplayName("Thời gian tạo")]
        public DateTime? CreateAt { get; set; }
        public virtual ICollection<Anhdaidien> Anhdaidiens { get; set; } = new List<Anhdaidien>();

        public virtual ICollection<Soyeulylich> Soyeulyliches { get; set; } = new List<Soyeulylich>();

        public virtual ICollection<Baituyendung> Baituyendungs { get; set; } = new List<Baituyendung>();

        public virtual ICollection<Congty> Congties { get; set; } = new List<Congty>();

        public virtual Loaitaikhoan? IdLtkNavigation { get; set; }

        public virtual Taikhoan? IdTkNavigation { get; set; }
    }
}
