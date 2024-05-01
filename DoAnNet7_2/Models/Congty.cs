using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DoAnNet7_2.Models;

public partial class Congty
{
    public int IdCt { get; set; }

    public string? Logo { get; set; }

    [DisplayName("Tên công ty")]
    public string Tencongty { get; set; } = null!;

    [DisplayName("Ngành nghề")]
    public int? IdNn { get; set; }

    [DisplayName("Số nhân viên")]

    public string? Sonhanvien { get; set; }

    [DisplayName("Địa chỉ")]

    public string? Diachi { get; set; }

    [DisplayName("Tỉnh thành")]
    public int? IdTt { get; set; }

    [DisplayName("Mô tả")]
    public string? Mota { get; set; }

    public int? IdTk { get; set; }

    public virtual Nganhnghe? IdNnNavigation { get; set; }

    public virtual Taikhoan? IdTkNavigation { get; set; }

    public virtual Tinhthanh? IdTtNavigation { get; set; }

    [NotMapped]

    [Display(Name = "Logo")]
    public IFormFile UpLogo { get; set; }
}
