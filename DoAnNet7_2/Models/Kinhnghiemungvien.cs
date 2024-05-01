using System;
using System.Collections.Generic;

namespace DoAnNet7_2.Models;

public partial class Kinhnghiemungvien
{
    public int IdKn { get; set; }

    public string Tenkn { get; set; } = null!;

    public string? Congty { get; set; }

    public string? Diachicongty { get; set; }

    public string? Batdau { get; set; }

    public string? Ketthuc { get; set; }

    public string? Mota { get; set; }

    public int? IdTk { get; set; }

    public virtual Taikhoan? IdTkNavigation { get; set; }
}
