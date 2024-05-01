using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DoAnNet7_2.Models;

public partial class Nganhnghe
{
    public int IdNn { get; set; }

    [DisplayName("Tên ngành nghề")]
    public string? Tennganhnghe { get; set; }

    public virtual ICollection<Baituyendung> Baituyendungs { get; set; } = new List<Baituyendung>();

    public virtual ICollection<Congty> Congties { get; set; } = new List<Congty>();
}
