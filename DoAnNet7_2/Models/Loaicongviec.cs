using System;
using System.Collections.Generic;

namespace DoAnNet7_2.Models;

public partial class Loaicongviec
{
    public int IdLcv { get; set; }

    public string? Tenlcv { get; set; }

    public virtual ICollection<Baituyendung> Baituyendungs { get; set; } = new List<Baituyendung>();
}
