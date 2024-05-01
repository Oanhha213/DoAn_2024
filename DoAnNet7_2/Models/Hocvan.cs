using System;
using System.Collections.Generic;

namespace DoAnNet7_2.Models;

public partial class Hocvan
{
    public int IdHv { get; set; }

    public string? Tentruong { get; set; }

    public string? Nganh { get; set; }

    public string? Loaibang { get; set; }

    public string? Mota { get; set; }

    public string? Thoigianbatdau { get; set; }

    public string? Thoigianketthuc { get; set; }

    public int? IdTk { get; set; }

    public virtual Taikhoan? IdTkNavigation { get; set; }
}
