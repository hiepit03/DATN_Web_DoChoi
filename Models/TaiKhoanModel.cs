using System.ComponentModel.DataAnnotations;

namespace Web_DoChoi.Models
{
    public class TaiKhoanModel
    {
        [Key]
        public int PK_TaiKhoan { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ChucVu { get; set; }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

    }
}
