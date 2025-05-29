using Microsoft.EntityFrameworkCore;
using Web_DoChoi.Models;

namespace Web_DoChoi.Repository
{
    public class SeedData
    {

        public static void SeedingData(DataContext _context)
        {
            _context.Database.Migrate();
            if (!_context.DoChois.Any())
            {
                DanhMucModel dm = new DanhMucModel {TenDanhMuc = "Đồ Chơi Xếp Hình", MoTa = "check" };
                _context.DanhMucs.Add(dm);

                NhaSanXuatModel nsx = new NhaSanXuatModel { TenNSX = "LEGO", MoTa = "check" };
                _context.NhaSanXuats.Add(nsx);

                DateTime dtime = DateTime.Now;
                DoChoiModel dc = new DoChoiModel
                {
                    TenDoChoi = "Lego check",
                    AnhDoChoi = "lego1.png",
                    GiaNhapHang = 50000,
                    GiaBan = 100000,
                    PhanTramKhuyenMai = 20,
                    SoLuongDaBan = 20,
                    SoLuongConLai = 100,
                    MoTa = "check",
                    NgayNhapHang = dtime,
                    DoTuoiGioiHan = 12,
                    SoSao = 4,
                    SoLuotDanhGia = 1,
                    NhaSanXuat = nsx,
                    DanhMuc = dm
                };
                _context.DoChois.Add(dc);

                TaiKhoanModel tk = new TaiKhoanModel
                {
                    UserName = "Admin",
                    Email = "admin@gmail.com",
                    ChucVu = "admin",
                    Password = "$2a$11$bkOFMIUcHmzwdgcvjDkikemX8VUzXUTNtrw4Pj28vk7lpheqqPwKC"
                };
                _context.TaiKhoans.Add(tk);

                MaKhuyenMaiModel mkm = new MaKhuyenMaiModel
                {
                    TenMaGiamGia = "Không Áp Dụng",
                    MoTa = "Không Áp Dụng",
                    GiaTienApDung = 0,
                    GiaTienGiam = 0,
                    ThoiGianBatDau = DateTime.MinValue,
                    ThoiGianKetThuc = DateTime.MinValue,
                };
                _context.MaKhuyenMais.Add(mkm);

                _context.SaveChanges();
            }

        }
    }
}
