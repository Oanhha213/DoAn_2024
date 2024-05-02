using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DoAnNet7_2.Models;

public partial class Jobsworld2Context : DbContext
{
    public Jobsworld2Context()
    {
    }

    public Jobsworld2Context(DbContextOptions<Jobsworld2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Anhdaidien> Anhdaidiens { get; set; }

    public virtual DbSet<Baituyendung> Baituyendungs { get; set; }

    public virtual DbSet<Chungchi> Chungchis { get; set; }

    public virtual DbSet<Congty> Congties { get; set; }

    public virtual DbSet<CongviecSyll> CongviecSylls { get; set; }

    public virtual DbSet<Hocvan> Hocvans { get; set; }

    public virtual DbSet<Hosocanhan> Hosocanhans { get; set; }

    public virtual DbSet<Kinhnghiemungvien> Kinhnghiemungviens { get; set; }

    public virtual DbSet<Kynang> Kynangs { get; set; }

    public virtual DbSet<Loaicongviec> Loaicongviecs { get; set; }

    public virtual DbSet<Loaitaikhoan> Loaitaikhoans { get; set; }

    public virtual DbSet<Thichcongviec> Thichcongviecs { get; set; }

    public virtual DbSet<Nganhnghe> Nganhnghes { get; set; }

    public virtual DbSet<Soyeulylich> Soyeulyliches { get; set; }

    public virtual DbSet<Taikhoan> Taikhoans { get; set; }

    public virtual DbSet<Thanhtuu> Thanhtuus { get; set; }

    public virtual DbSet<Thoigiankinhnghiem> Thoigiankinhnghiems { get; set; }
    public virtual DbSet<Thoigianlamviec> Thoigianlamviecs { get; set; }

    public virtual DbSet<Tinhthanh> Tinhthanhs { get; set; }

    public virtual DbSet<Trangthaihienthi> Trangthaihienthis { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-7JF815G;Initial Catalog=JOBSWORLD2;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Anhdaidien>(entity =>
        {
            entity.HasKey(e => e.IdAnhdd).HasName("PK__ANHDAIDI__0F96E2FE84975FE2");

            entity.ToTable("ANHDAIDIEN");

            entity.Property(e => e.IdAnhdd).HasColumnName("ID_ANHDD");
            entity.Property(e => e.IdTk).HasColumnName("ID_TK");
            entity.Property(e => e.Tenanh)
                .HasColumnType("ntext")
                .HasColumnName("TENANH");

            entity.HasOne(d => d.IdTkNavigation).WithMany(p => p.Anhdaidiens)
                .HasForeignKey(d => d.IdTk)
                .HasConstraintName("FK__ANHDAIDIE__ID_TK__44FF419A");
        });

        modelBuilder.Entity<Baituyendung>(entity =>
        {
            entity.HasKey(e => e.IdBtd).HasName("PK__BAITUYEN__143B6E1F24F4CD7C");

            entity.ToTable("BAITUYENDUNG");

            entity.Property(e => e.IdBtd).HasColumnName("ID_BTD");
            entity.Property(e => e.Diachi).HasColumnName("DIACHI");
            entity.Property(e => e.IdTtht).HasColumnName("ID_TTHT");
            entity.Property(e => e.IdLcv).HasColumnName("ID_LCV");
            entity.Property(e => e.IdNn).HasColumnName("ID_NN");
            entity.Property(e => e.IdTgkn).HasColumnName("ID_TGKN");
            entity.Property(e => e.IdTk).HasColumnName("ID_TK");
            entity.Property(e => e.IdTt).HasColumnName("ID_TT");
            entity.Property(e => e.IdTglv).HasColumnName("ID_TGLV");
            entity.Property(e => e.Motacongviec)
                .HasColumnType("ntext")
                .HasColumnName("MOTACONGVIEC");
            entity.Property(e => e.Quyenloi)
                .HasColumnType("ntext")
                .HasColumnName("QUYENLOI");
            entity.Property(e => e.Tencongviec)
                .HasMaxLength(250)
                .HasColumnName("TENCONGVIEC");
            entity.Property(e => e.Yeucaukhac)
                .HasColumnType("ntext")
                .HasColumnName("YEUCAUKHAC");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_AT");

            entity.HasOne(d => d.IdLcvNavigation).WithMany(p => p.Baituyendungs)
                .HasForeignKey(d => d.IdLcv)
                .HasConstraintName("FK__BAITUYEND__ID_LC__5070F446");

            entity.HasOne(d => d.IdNnNavigation).WithMany(p => p.Baituyendungs)
                .HasForeignKey(d => d.IdNn)
                .HasConstraintName("FK__BAITUYEND__ID_NN__4F7CD00D");

            entity.HasOne(d => d.IdTgknNavigation).WithMany(p => p.Baituyendungs)
                .HasForeignKey(d => d.IdTgkn)
                .HasConstraintName("FK__BAITUYEND__ID_TG__5165187F");

            entity.HasOne(d => d.IdTkNavigation).WithMany(p => p.Baituyendungs)
                .HasForeignKey(d => d.IdTk)
                .HasConstraintName("FK__BAITUYEND__ID_TK__534D60F1");

            entity.HasOne(d => d.IdTtNavigation).WithMany(p => p.Baituyendungs)
                .HasForeignKey(d => d.IdTt)
                .HasConstraintName("FK__BAITUYEND__ID_TT__52593CB8");
            entity.HasOne(d => d.IdTglvNavigation).WithMany(p => p.Baituyendungs)
                .HasForeignKey(d => d.IdTglv)
                .HasConstraintName("FK__BAITUYEND__ID_TG__6E01572D");
            entity.HasOne(d => d.IdTthtNavigation).WithMany(p => p.Baituyendungs)
                .HasForeignKey(d => d.IdTtht)
                .HasConstraintName("FK__BAITUYEND__ID_TT__02FC7413");
        });

        modelBuilder.Entity<Chungchi>(entity =>
        {
            entity.HasKey(e => e.IdCc).HasName("PK__CHUNGCHI__8B622F8C5FAEA2A8");

            entity.ToTable("CHUNGCHI");

            entity.Property(e => e.IdCc).HasColumnName("ID_CC");
            entity.Property(e => e.IdTk).HasColumnName("ID_TK");
            entity.Property(e => e.Tenchungchi).HasColumnName("TENCHUNGCHI");
            entity.Property(e => e.Thoigiancap).HasColumnName("THOIGIANCAP");
            entity.Property(e => e.Thoigianhethan).HasColumnName("THOIGIANHETHAN");

            entity.HasOne(d => d.IdTkNavigation).WithMany(p => p.Chungchis)
                .HasForeignKey(d => d.IdTk)
                .HasConstraintName("FK__CHUNGCHI__ID_TK__5EBF139D");
        });

        modelBuilder.Entity<Congty>(entity =>
        {
            entity.HasKey(e => e.IdCt).HasName("PK__CONGTY__8B622F9F96A78A69");

            entity.ToTable("CONGTY");

            entity.Property(e => e.IdCt).HasColumnName("ID_CT");
            entity.Property(e => e.Diachi).HasColumnName("DIACHI");
            entity.Property(e => e.IdNn).HasColumnName("ID_NN");
            entity.Property(e => e.IdTk).HasColumnName("ID_TK");
            entity.Property(e => e.IdTt).HasColumnName("ID_TT");
            entity.Property(e => e.Logo).HasColumnName("LOGO");
            entity.Property(e => e.Mota)
                .HasColumnType("ntext")
                .HasColumnName("MOTA");
            entity.Property(e => e.Sonhanvien).HasColumnName("SONHANVIEN");
            entity.Property(e => e.Tencongty).HasColumnName("TENCONGTY");

            entity.HasOne(d => d.IdNnNavigation).WithMany(p => p.Congties)
                .HasForeignKey(d => d.IdNn)
                .HasConstraintName("FK__CONGTY__ID_NN__4AB81AF0");

            entity.HasOne(d => d.IdTkNavigation).WithMany(p => p.Congties)
                .HasForeignKey(d => d.IdTk)
                .HasConstraintName("FK__CONGTY__ID_TK__4CA06362");

            entity.HasOne(d => d.IdTtNavigation).WithMany(p => p.Congties)
                .HasForeignKey(d => d.IdTt)
                .HasConstraintName("FK__CONGTY__ID_TT__4BAC3F29");
        });

        modelBuilder.Entity<CongviecSyll>(entity =>
        {
            entity.HasKey(e => e.IdCvsyll).HasName("PK__CONGVIEC__7B44EE6F1F708613");

            entity.ToTable("CONGVIEC_SYLL");

            entity.Property(e => e.IdCvsyll).HasColumnName("ID_CVSYLL");
            entity.Property(e => e.IdBtd).HasColumnName("ID_BTD");
            entity.Property(e => e.IdSyll).HasColumnName("ID_SYLL");

            entity.HasOne(d => d.IdBtdNavigation).WithMany(p => p.CongviecSylls)
                .HasForeignKey(d => d.IdBtd)
                .HasConstraintName("FK__CONGVIEC___ID_BT__6754599E");

            entity.HasOne(d => d.IdSyllNavigation).WithMany(p => p.CongviecSylls)
                .HasForeignKey(d => d.IdSyll)
                .HasConstraintName("FK__CONGVIEC___ID_SY__68487DD7");
        });

        modelBuilder.Entity<Hocvan>(entity =>
        {
            entity.HasKey(e => e.IdHv).HasName("PK__HOCVAN__8B62D7330BB7739C");

            entity.ToTable("HOCVAN");

            entity.Property(e => e.IdHv).HasColumnName("ID_HV");
            entity.Property(e => e.IdTk).HasColumnName("ID_TK");
            entity.Property(e => e.Loaibang)
                .HasMaxLength(250)
                .HasColumnName("LOAIBANG");
            entity.Property(e => e.Mota).HasColumnName("MOTA");
            entity.Property(e => e.Nganh)
                .HasMaxLength(300)
                .HasColumnName("NGANH");
            entity.Property(e => e.Tentruong)
                .HasMaxLength(300)
                .HasColumnName("TENTRUONG");
            entity.Property(e => e.Thoigianbatdau).HasColumnName("THOIGIANBATDAU");
            entity.Property(e => e.Thoigianketthuc).HasColumnName("THOIGIANKETTHUC");

            entity.HasOne(d => d.IdTkNavigation).WithMany(p => p.Hocvans)
                .HasForeignKey(d => d.IdTk)
                .HasConstraintName("FK__HOCVAN__ID_TK__59063A47");
        });

        modelBuilder.Entity<Hosocanhan>(entity =>
        {
            entity.HasKey(e => e.IdHscn).HasName("PK__HOSOCANH__73B4A3CAA42BAA6F");

            entity.ToTable("HOSOCANHAN");

            entity.Property(e => e.IdHscn).HasColumnName("ID_HSCN");
            entity.Property(e => e.Anhhoso).HasColumnName("ANHHOSO");
            entity.Property(e => e.Congviecmongmuon)
                .HasMaxLength(250)
                .HasColumnName("CONGVIECMONGMUON");
            entity.Property(e => e.Diachi).HasColumnName("DIACHI");
            entity.Property(e => e.IdTk).HasColumnName("ID_TK");
            entity.Property(e => e.Loigioithieu)
                .HasColumnType("ntext")
                .HasColumnName("LOIGIOITHIEU");
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.Tendaydu)
                .HasMaxLength(250)
                .HasColumnName("TENDAYDU");

            entity.HasOne(d => d.IdTkNavigation).WithMany(p => p.Hosocanhans)
                .HasForeignKey(d => d.IdTk)
                .HasConstraintName("FK__HOSOCANHA__ID_TK__47DBAE45");
        });

        modelBuilder.Entity<Kinhnghiemungvien>(entity =>
        {
            entity.HasKey(e => e.IdKn).HasName("PK__KINHNGHI__8B62EC8FFDF0CEFF");

            entity.ToTable("KINHNGHIEMUNGVIEN");

            entity.Property(e => e.IdKn).HasColumnName("ID_KN");
            entity.Property(e => e.Batdau)
                .HasMaxLength(250)
                .HasColumnName("BATDAU");
            entity.Property(e => e.Congty).HasColumnName("CONGTY");
            entity.Property(e => e.Diachicongty).HasColumnName("DIACHICONGTY");
            entity.Property(e => e.IdTk).HasColumnName("ID_TK");
            entity.Property(e => e.Ketthuc)
                .HasMaxLength(250)
                .HasColumnName("KETTHUC");
            entity.Property(e => e.Mota)
                .HasColumnType("ntext")
                .HasColumnName("MOTA");
            entity.Property(e => e.Tenkn)
                .HasColumnType("ntext")
                .HasColumnName("TENKN");

            entity.HasOne(d => d.IdTkNavigation).WithMany(p => p.Kinhnghiemungviens)
                .HasForeignKey(d => d.IdTk)
                .HasConstraintName("FK__KINHNGHIE__ID_TK__5629CD9C");
        });

        modelBuilder.Entity<Kynang>(entity =>
        {
            entity.HasKey(e => e.IdKn).HasName("PK__KYNANG__8B62EC8F649D41CD");

            entity.ToTable("KYNANG");

            entity.Property(e => e.IdKn).HasColumnName("ID_KN");
            entity.Property(e => e.IdTk).HasColumnName("ID_TK");
            entity.Property(e => e.Tenkn).HasColumnName("TENKN");

            entity.HasOne(d => d.IdTkNavigation).WithMany(p => p.Kynangs)
                .HasForeignKey(d => d.IdTk)
                .HasConstraintName("FK__KYNANG__ID_TK__5BE2A6F2");
        });

        modelBuilder.Entity<Loaicongviec>(entity =>
        {
            entity.HasKey(e => e.IdLcv).HasName("PK__LOAICONG__2DBE951D40D31CAA");

            entity.ToTable("LOAICONGVIEC");

            entity.Property(e => e.IdLcv).HasColumnName("ID_LCV");
            entity.Property(e => e.Tenlcv)
                .HasMaxLength(250)
                .HasColumnName("TENLCV");
        });

        modelBuilder.Entity<Loaitaikhoan>(entity =>
        {
            entity.HasKey(e => e.IdLtk).HasName("PK__LOAITAIK__2DBE08ACD8BB70C0");

            entity.ToTable("LOAITAIKHOAN");

            entity.Property(e => e.IdLtk).HasColumnName("ID_LTK");
            entity.Property(e => e.Tenltk)
                .HasMaxLength(50)
                .HasColumnName("TENLTK");
        });

        modelBuilder.Entity<Thichcongviec>(entity =>
        {
            entity.HasKey(e => e.IdTcv).HasName("PK__THICHCON__27BF13734AF4FA49");
            entity.ToTable("THICHCONGVIEC");
            entity.Property(e => e.IdTcv).HasColumnName("ID_TCV");
            entity.Property(e => e.IdBtd).HasColumnName("ID_BTD");
            entity.Property(e => e.IdTk).HasColumnName("ID_TK");

            entity.HasOne(d => d.IdBtdNavigation).WithMany(p => p.Thichcongviecs)
                .HasForeignKey(d => d.IdBtd)
                .HasConstraintName("FK__THICHCONG__ID_BT__06CD04F7");

            entity.HasOne(d => d.IdTkNavigation).WithMany(p => p.Thichcongviecs)
                .HasForeignKey(d => d.IdTk)
                .HasConstraintName("FK__THICHCONG__ID_TK__07C12930");
        });

        modelBuilder.Entity<Nganhnghe>(entity =>
        {
            entity.HasKey(e => e.IdNn).HasName("PK__NGANHNGH__8B63E06BE7DFEBF9");

            entity.ToTable("NGANHNGHE");

            entity.Property(e => e.IdNn).HasColumnName("ID_NN");
            entity.Property(e => e.Tennganhnghe).HasColumnName("TENNGANHNGHE");
        });

        modelBuilder.Entity<Soyeulylich>(entity =>
        {
            entity.HasKey(e => e.IdSyll).HasName("PK__SOYEULYL__FEF7EA05567D0380");

            entity.ToTable("SOYEULYLICH");

            entity.Property(e => e.IdSyll).HasColumnName("ID_SYLL");
            entity.Property(e => e.IdTk).HasColumnName("ID_TK");
            entity.Property(e => e.Tensyll)
                .HasMaxLength(250)
                .HasColumnName("TENSYLL");
            entity.Property(e => e.Duongdansyll)
                .HasColumnType("ntext")
                .HasColumnName("DUONGDANSYLL");

            entity.HasOne(d => d.IdTkNavigation).WithMany(p => p.Soyeulyliches)
                .HasForeignKey(d => d.IdTk)
                .HasConstraintName("FK__SOYEULYLI__ID_TK__6477ECF3");
        });

        modelBuilder.Entity<Taikhoan>(entity =>
        {
            entity.HasKey(e => e.IdTk).HasName("PK__TAIKHOAN__8B63B1A94CD2B84E");

            entity.ToTable("TAIKHOAN");

            entity.HasIndex(e => e.Email, "UQ__TAIKHOAN__161CF7248F633061").IsUnique();

            entity.Property(e => e.IdTk).HasColumnName("ID_TK");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Hoten)
                .HasMaxLength(255)
                .HasColumnName("HOTEN");
            entity.Property(e => e.IdLtk).HasColumnName("ID_LTK");
            entity.Property(e => e.Madatlaimatkhau)
                .IsUnicode(false)
                .HasColumnName("MADATLAIMATKHAU");
            entity.Property(e => e.Matkhau)
                .HasMaxLength(255)
                .HasColumnName("MATKHAU");

            entity.HasOne(d => d.IdLtkNavigation).WithMany(p => p.Taikhoans)
                .HasForeignKey(d => d.IdLtk)
                .HasConstraintName("FK__TAIKHOAN__ID_LTK__4222D4EF");
            entity.Property(e => e.Sdt)
                .HasMaxLength(12)
                .HasColumnName("SDT");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_AT");
        });

        modelBuilder.Entity<Thanhtuu>(entity =>
        {
            entity.HasKey(e => e.IdThanhtuu).HasName("PK__THANHTUU__F687A9157895AE84");

            entity.ToTable("THANHTUU");

            entity.Property(e => e.IdThanhtuu).HasColumnName("ID_THANHTUU");
            entity.Property(e => e.IdTk).HasColumnName("ID_TK");
            entity.Property(e => e.Mota)
                .HasColumnType("ntext")
                .HasColumnName("MOTA");
            entity.Property(e => e.Tenthanhtuu).HasColumnName("TENTHANHTUU");

            entity.HasOne(d => d.IdTkNavigation).WithMany(p => p.Thanhtuus)
                .HasForeignKey(d => d.IdTk)
                .HasConstraintName("FK__THANHTUU__ID_TK__619B8048");
        });

        modelBuilder.Entity<Thoigiankinhnghiem>(entity =>
        {
            entity.HasKey(e => e.IdTgkn).HasName("PK__THOIGIAN__DACDFCFCBAA47B12");

            entity.ToTable("THOIGIANKINHNGHIEM");

            entity.Property(e => e.IdTgkn).HasColumnName("ID_TGKN");
            entity.Property(e => e.Tentgkn)
                .HasMaxLength(250)
                .HasColumnName("TENTGKN");
        });

        modelBuilder.Entity<Thoigianlamviec>(entity =>
        {
            entity.HasKey(e => e.IdTglv).HasName("PK__THOIGIAN__DACDE415F7B5FCA0");

            entity.ToTable("THOIGIANLAMVIEC");

            entity.Property(e => e.IdTglv).HasColumnName("ID_TGLV");
            entity.Property(e => e.Tentglv)
                .HasMaxLength(250)
                .HasColumnName("TENTGLV");
        });

        modelBuilder.Entity<Tinhthanh>(entity =>
        {
            entity.HasKey(e => e.IdTt).HasName("PK__TINHTHAN__8B63B1A2D5884A5F");

            entity.ToTable("TINHTHANH");

            entity.Property(e => e.IdTt).HasColumnName("ID_TT");
            entity.Property(e => e.Tentt)
                .HasMaxLength(150)
                .HasColumnName("TENTT");
        });
        modelBuilder.Entity<Trangthaihienthi>(entity =>
        {
            entity.HasKey(e => e.IdTtht).HasName("PK__TRANGTHA__D99918529D76754C");

            entity.ToTable("TRANGTHAIHIENTHI");

            entity.Property(e => e.IdTtht).HasColumnName("ID_TTHT");
            entity.Property(e => e.Tenttht)
                .HasMaxLength(50)
                .HasColumnName("TENTTHT");
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
