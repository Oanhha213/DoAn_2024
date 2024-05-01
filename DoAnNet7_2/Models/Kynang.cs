using System;
using System.Collections.Generic;

namespace DoAnNet7_2.Models;

public partial class Kynang
{
    public int IdKn { get; set; }

    public string Tenkn { get; set; } = null!;

    public int? IdTk { get; set; }

    public virtual Taikhoan? IdTkNavigation { get; set; }
}
