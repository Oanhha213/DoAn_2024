using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DoAnNet7_2.Models;

public partial class Trangthaihienthi
{
    public int IdTtht { get; set; }

    [DisplayName("Tên trạng thái hiển thị")]
    public string? Tenttht { get; set; }

    public virtual ICollection<Baituyendung> Baituyendungs { get; set; } = new List<Baituyendung>();

}
