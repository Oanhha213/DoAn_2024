using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DoAnNet7_2.Models;

public partial class Soyeulylich
{
    [DisplayName("ID")]
    public int IdSyll { get; set; }

    [DisplayName("Tên CV")]
    public string? Tensyll { get; set; }

    [DisplayName("Đường dẫn CV")]
    public string? Duongdansyll { get; set; }

    [DisplayName("Tài khoản")]
    public int? IdTk { get; set; }

    public virtual ICollection<CongviecSyll> CongviecSylls { get; set; } = new List<CongviecSyll>();

    public virtual Taikhoan? IdTkNavigation { get; set; }

    [NotMapped]

    [Display(Name = "CV")]
    public IFormFile cvFile { get; set; }
}
