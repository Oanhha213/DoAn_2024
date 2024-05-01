using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DoAnNet7_2.Models;

public partial class Anhdaidien
{
    public int IdAnhdd { get; set; }

    [DisplayName("Tên ảnh")]
    public string? Tenanh { get; set; }

    [DisplayName("Tài khoản")]
    public int? IdTk { get; set; }

    public virtual Taikhoan? IdTkNavigation { get; set; }

    [NotMapped]

    [Display(Name = "Ảnh đại diện")]
    public IFormFile UpAnhDaiDien { get; set; }
}
