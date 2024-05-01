using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DoAnNet7_2.Models;

public partial class CongviecSyll
{
    public int IdCvsyll { get; set; }

    [DisplayName("Bài tuyển dụng")]
    public int? IdBtd { get; set; }

    [DisplayName("Sơ yếu lý lịch")]
    public int? IdSyll { get; set; }

    public virtual Baituyendung? IdBtdNavigation { get; set; }

    public virtual Soyeulylich? IdSyllNavigation { get; set; }
}
