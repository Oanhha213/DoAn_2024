using System;
using System.Collections.Generic;

namespace DoAnNet7_2.Models
{
    public class Luong
    {
        public int IdLuong { get; set; }

        public string Tenmucluong { get; set; } = null!;

        public virtual ICollection<Baituyendung> Baituyendungs { get; set; } = new List<Baituyendung>();

    }
}
