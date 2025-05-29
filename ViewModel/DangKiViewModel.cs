using System.ComponentModel.DataAnnotations;

namespace Web_DoChoi.ViewModel
{
    public class DangKiViewModel
    {
        [Required(ErrorMessage = "Tên hiển thị là bắt buộc.")]
        [MinLength(6, ErrorMessage = "Tên hiển thị phải có ít nhất 6 ký tự.")]
        [MaxLength(30, ErrorMessage = "Tên hiển thị không được vượt quá 20 ký tự.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string RePassword { get; set; }
    }
}
