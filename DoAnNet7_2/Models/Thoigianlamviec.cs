using System;
using System.Collections.Generic;

namespace DoAnNet7_2.Models;

public partial class Thoigianlamviec
{
    public int IdTglv { get; set; }

    public string? Tentglv { get; set; }

    public virtual ICollection<Baituyendung> Baituyendungs { get; set; } = new List<Baituyendung>();
}
