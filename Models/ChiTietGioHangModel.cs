using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_DoChoi.Models
{
    public class ChiTietGioHangModel
    {
        [Key]
        public int PK_ChiTietGioHang { get; set; }
        public int SoLuongSP {  get; set; }

        [Required]
        public int FK_DoChoi { get; set; }

        [ForeignKey("FK_DoChoi")]
        public DoChoiModel DoChoi { get; set; }

        public int FK_GioHang { get; set; }

        [ForeignKey("FK_GioHang")]
        public GioHangModel GioHang { get; set; }
    }
}
