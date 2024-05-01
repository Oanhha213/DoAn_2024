using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DoAnNet7_2.Models;

public partial class Tinhthanh
{
    public int IdTt { get; set; }

    [DisplayName("Tên tỉnh thành")]
    public string? Tentt { get; set; }

    public virtual ICollection<Baituyendung> Baituyendungs { get; set; } = new List<Baituyendung>();

    public virtual ICollection<Congty> Congties { get; set; } = new List<Congty>();
}
