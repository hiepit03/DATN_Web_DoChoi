using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_DoChoi.Models
{
    public class GioHangModel
    {
        [Key]
        public int PK_GioHang { get; set; }
        [Required]
        public int FK_TaiKhoan { get; set; }

        [ForeignKey("FK_TaiKhoan")]
        public TaiKhoanModel TaiKhoan { get; set; }
    }
}
