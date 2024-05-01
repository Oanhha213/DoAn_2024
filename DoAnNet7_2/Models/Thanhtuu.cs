using System;
using System.Collections.Generic;

namespace DoAnNet7_2.Models;

public partial class Thanhtuu
{
    public int IdThanhtuu { get; set; }

    public string Tenthanhtuu { get; set; } = null!;

    public string? Mota { get; set; }

    public int? IdTk { get; set; }

    public virtual Taikhoan? IdTkNavigation { get; set; }
}
