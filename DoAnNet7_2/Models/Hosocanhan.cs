using System;
using System.Collections.Generic;

namespace DoAnNet7_2.Models;

public partial class Hosocanhan
{
    public int IdHscn { get; set; }

    public string Tendaydu { get; set; } = null!;

    public string Diachi { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public string Anhhoso { get; set; } = null!;

    public string? Congviecmongmuon { get; set; }

    public string? Loigioithieu { get; set; }

    public int? IdTk { get; set; }

    public virtual Taikhoan? IdTkNavigation { get; set; }
}
