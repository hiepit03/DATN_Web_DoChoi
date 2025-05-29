using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_DoChoi.Models
{
    public class DiaChiModel
    {
        [Key]
        public int PK_DiaChi { get; set; }
        public string DiaChiCuThe { get; set; }
        public string SDTNhanHang { get; set; }
        public string TenNguoiNhan { get; set; }
        public bool MacDinh { get; set; } = true;

        public int FK_TaiKhoan { get; set; }

        [ForeignKey("FK_TaiKhoan")]
        public TaiKhoanModel? TaiKhoan { get; set; }
    }
}
