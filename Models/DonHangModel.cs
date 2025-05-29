using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_DoChoi.Models
{
    public class DonHangModel
    {
        [Key]
        public int PK_DonHang { get; set; }
        public DateTime ThoiGianDatHang { get; set; }
        public string TrangThaiDonHang {  get; set; }
        public string? GhiChuDonHang { get; set; }
        public string DonViVanChuyen { get; set; }
        public DateTime ThoiGianGiaoHangDuKien { get; set; }
        public int TongGiaTri {  get; set; }
        public int LoiNhuan {  get; set; }
        public string PhuongThucThanhToan {  get; set; }
        public bool TrangThaiThanhToan { get; set; } = false;  
        public string NguoiNhan { get; set; }
        public string SDT {  get; set; }
        public string DiaChi {  get; set; }

        public int FK_MaKhuyenMai { get; set; }

        [ForeignKey("FK_MaKhuyenMai")]
        public MaKhuyenMaiModel? MaKhuyenMai { get; set; }

        public int FK_TaiKhoan { get; set; }

        [ForeignKey("FK_TaiKhoan")]
        public TaiKhoanModel? TaiKhoan { get; set; }
    }
}
