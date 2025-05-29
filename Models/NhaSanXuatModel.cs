using System.ComponentModel.DataAnnotations;

namespace Web_DoChoi.Models
{
    public class NhaSanXuatModel
    {
        [Key]
        public int PK_NSX { get; set; }

        [Required]
        [StringLength(255)]
        public string TenNSX { get; set; }

        public string MoTa { get; set; }

        public bool Visible { get; set; } = true; // Mặc định là hiển thị
    }
}
