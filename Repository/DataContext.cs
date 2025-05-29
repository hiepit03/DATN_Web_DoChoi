using Microsoft.EntityFrameworkCore;
using Web_DoChoi.Models;

namespace Web_DoChoi.Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<ChiNhanhModel> ChiNhanhs { get; set; }
        public DbSet<ChiTietDonHangModel> ChiTietDonHangs { get; set; }
        public DbSet<ChiTietGioHangModel> ChiTietGioHangs { get; set; }
        public DbSet<DanhGiaModel> DanhGias { get; set; }
        public DbSet<DanhMucModel> DanhMucs { get; set; }
        public DbSet<DiaChiModel> DiaChis { get; set; }
        public DbSet<DoChoiModel> DoChois { get; set; }
        public DbSet<DonHangModel> DonHangs { get; set; }
        public DbSet<GioHangModel> GioHangs { get; set; }
        public DbSet<LienHeModel> LienHes { get; set; }
        public DbSet<MaKhuyenMaiModel> MaKhuyenMais { get; set; }
        public DbSet<NhaSanXuatModel> NhaSanXuats { get; set; }
        public DbSet<SlideModel> Slides { get; set; }
        public DbSet<TaiKhoanModel> TaiKhoans { get; set; }
        public DbSet<TinTucModel> TinTucs { get; set; }
        public DbSet<OrderInfoModel> OrderInfos {  get; set; }

    }
}
