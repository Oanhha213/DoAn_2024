using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DoAnNet7_2.Models;

public partial class Taikhoan
{
    [DisplayName("Tài khoản")]
    public int IdTk { get; set; }

    [DisplayName("Họ tên")]
    public string Hoten { get; set; } = null!;

    public string Email { get; set; } = null!;

    [DisplayName("Mật khẩu")]
    public string Matkhau { get; set; } = null!;

    [DisplayName("Số điện thoại")]
    public string? Sdt { get; set; }

    [DisplayName("Loại tài khoản")]
    public int? IdLtk { get; set; }

    public string? Madatlaimatkhau { get; set; }

    [DisplayName("Thời gian tạo")]
    public DateTime? CreateAt { get; set; }
    public virtual ICollection<Anhdaidien> Anhdaidiens { get; set; } = new List<Anhdaidien>();

    public virtual ICollection<Baituyendung> Baituyendungs { get; set; } = new List<Baituyendung>();
    public virtual ICollection<Thichcongviec> Thichcongviecs { get; set; } = new List<Thichcongviec>();

    public virtual ICollection<Chungchi> Chungchis { get; set; } = new List<Chungchi>();

    public virtual ICollection<Congty> Congties { get; set; } = new List<Congty>();

    public virtual ICollection<Hocvan> Hocvans { get; set; } = new List<Hocvan>();

    public virtual ICollection<Hosocanhan> Hosocanhans { get; set; } = new List<Hosocanhan>();

    public virtual Loaitaikhoan? IdLtkNavigation { get; set; }

    public virtual ICollection<Kinhnghiemungvien> Kinhnghiemungviens { get; set; } = new List<Kinhnghiemungvien>();

    public virtual ICollection<Kynang> Kynangs { get; set; } = new List<Kynang>();

    public virtual ICollection<Soyeulylich> Soyeulyliches { get; set; } = new List<Soyeulylich>();

    public virtual ICollection<Thanhtuu> Thanhtuus { get; set; } = new List<Thanhtuu>();
}
