using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DoAnNet7_2.Models;

public partial class Thichcongviec
{
    public int IdTcv { get; set; }

    public int? IdBtd { get; set; }

    public int? IdTk { get; set; }

    public virtual Baituyendung? IdBtdNavigation { get; set; }

    public virtual Taikhoan? IdTkNavigation { get; set; }
}
