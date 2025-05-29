using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_DoChoi.Models
{
    public class DanhGiaModel
    {
        [Key]
        public int PK_DanhGia { get; set; }
        public int SoSaoDanhGia { get; set; }
        public string LoiDanhGia { get; set; }
        public DateTime ThoiGianThem { get; set; }
        public int FK_TaiKhoan { get; set; }

        [ForeignKey("FK_TaiKhoan")]
        public TaiKhoanModel TaiKhoan { get; set; }

        public int FK_DoChoi { get; set; }

        [ForeignKey("FK_DoChoi")]
        public DoChoiModel DoChoi { get; set; }

    }
}
