using System.ComponentModel;

namespace DoAnNet7_2.Models.ViewModels
{
    public class TaiKhoanCongTyViewModel
    {
        [DisplayName("Tài khoản")]
        public int IdTk { get; set; }

        public int IdCt { get; set; }

        [DisplayName("Họ tên")]
        public string Hoten { get; set; } = null!;

        public string Email { get; set; } = null!;

        [DisplayName("Mật khẩu")]
        public string Matkhau { get; set; } = null!;

        [DisplayName("Số điện thoại")]
        public string? Sdt { get; set; }

        public string? Logo { get; set; }

        [DisplayName("Tên công ty")]
        public string Tencongty { get; set; } = null!;

        [DisplayName("Loại tài khoản")]
        public int? IdLtk { get; set; }

        public virtual Loaitaikhoan? IdLtkNavigation { get; set; }


    }
}
