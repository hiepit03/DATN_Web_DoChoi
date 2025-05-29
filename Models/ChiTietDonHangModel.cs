using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_DoChoi.Models
{
    public class ChiTietDonHangModel
    {
        [Key]
        public int PK_ChiTietDonHang { get; set; }
        public int SoLuongSP { get; set; }

        [Required]
        public int FK_DoChoi { get; set; }

        [ForeignKey("FK_DoChoi")]
        public DoChoiModel DoChoi { get; set; }

        public int FK_DonHang { get; set; }

        [ForeignKey("FK_DonHang")]
        public DonHangModel DonHang { get; set; }
    }
}
