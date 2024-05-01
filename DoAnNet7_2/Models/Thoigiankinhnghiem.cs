using System;
using System.Collections.Generic;

namespace DoAnNet7_2.Models;

public partial class Thoigiankinhnghiem
{
    public int IdTgkn { get; set; }

    public string? Tentgkn { get; set; }

    public virtual ICollection<Baituyendung> Baituyendungs { get; set; } = new List<Baituyendung>();
}
