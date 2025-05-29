using System.ComponentModel.DataAnnotations;

namespace Web_DoChoi.Models
{
    public class DanhMucModel
    {
        [Key]
        public int PK_DanhMuc { get; set; }
        public string TenDanhMuc {get; set; }
        public string MoTa { get; set; }
        public bool Visible { get; set; } = true; // Mặc định là hiển thị
    }
}
