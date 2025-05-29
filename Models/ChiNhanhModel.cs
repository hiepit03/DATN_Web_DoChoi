using System.ComponentModel.DataAnnotations;

namespace Web_DoChoi.Models
{
    public class ChiNhanhModel
    {
        [Key]
        public int PK_ChiNhanh { get; set; }

        [Required]
        [StringLength(255)]
        public string TenChiNhanh { get; set; }

        public string DiaChi { get; set; } 

        public string GioiThieu { get; set; } 
    }
}
