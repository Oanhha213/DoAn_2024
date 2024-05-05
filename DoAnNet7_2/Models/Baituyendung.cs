using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DoAnNet7_2.Models;

public partial class Baituyendung
{
    [DisplayName("ID")]
    public int IdBtd { get; set; }

    [DisplayName("Tên công việc")]
    public string Tencongviec { get; set; } = null!;

    [DisplayName("Địa chỉ")]
    public string? Diachi { get; set; } = null!;

    [DisplayName("Trạng thái hiển thị")]
    public int IdTtht { get; set; }

    [DisplayName("Tỉnh thành")]
    public int IdTt { get; set; }

    [DisplayName("Loại công việc")]
    public int IdLcv { get; set; }

    [DisplayName("Thời gian làm việc")]
    public string Thoigianlamviec { get; set; } = null!;

    [DisplayName("Ngành nghề")]
    public int? IdNn { get; set; }

    [DisplayName("Mô tả công việc")]
    public string Motacongviec { get; set; } = null!;

    [DisplayName("Thời gian kinh nghiệm")]
    public int IdTgkn { get; set; }

    [DisplayName("Yêu cầu khác")]
    public string Yeucaukhac { get; set; } = null!;

    [DisplayName("Quyền lợi ")]
    public string Quyenloi { get; set; } = null!;

    [DisplayName("Tài khoản ")]
    public int? IdTk { get; set; }

    [DisplayName("Mức lương ")]
    public int IdLuong { get; set; }

    [DisplayName("Thời gian tạo bài")]
    public DateTime? CreateAt { get; set; }

    public virtual ICollection<CongviecSyll> CongviecSylls { get; set; } = new List<CongviecSyll>();
    public virtual ICollection<Thichcongviec> Thichcongviecs { get; set; } = new List<Thichcongviec>();
    public virtual Loaicongviec? IdLcvNavigation { get; set; }
    public virtual Luong? IdLuongNavigation { get; set; }

    public virtual Nganhnghe? IdNnNavigation { get; set; }

    public virtual Thoigiankinhnghiem? IdTgknNavigation { get; set; }

    public virtual Taikhoan? IdTkNavigation { get; set; }

    public virtual Tinhthanh? IdTtNavigation { get; set; }

    public virtual Trangthaihienthi? IdTthtNavigation { get; set; }
    //public object IdBtdNavigation { get; internal set; }
}
