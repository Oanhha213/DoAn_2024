using System;
using System.Collections.Generic;

namespace DoAnNet7_2.Models;

public partial class Loaitaikhoan
{
    public int IdLtk { get; set; }

    public string? Tenltk { get; set; }

    public virtual ICollection<Taikhoan> Taikhoans { get; set; } = new List<Taikhoan>();
}
