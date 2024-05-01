using System;
using System.Collections.Generic;

namespace DoAnNet7_2.Models;

public partial class Chungchi
{
    public int IdCc { get; set; }

    public string? Thoigiancap { get; set; }

    public string? Thoigianhethan { get; set; }

    public string Tenchungchi { get; set; } = null!;

    public int? IdTk { get; set; }

    public virtual Taikhoan? IdTkNavigation { get; set; }
}
