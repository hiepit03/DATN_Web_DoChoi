using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_DoChoi.Repository;
using Microsoft.AspNetCore.Hosting;
using Web_DoChoi.ViewModel;
using Web_DoChoi.Models;
using Web_DoChoi.Services;

namespace Web_DoChoi.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<TaiKhoanController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly EmailSender _emailSender;

        public TaiKhoanController(ILogger<TaiKhoanController> logger, DataContext dataContext, IWebHostEnvironment webHostEnvironment, EmailSender emailSender)
        {
            _logger = logger;
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (User.Identity.IsAuthenticated)
            {
                // Nếu đã đăng nhập rồi, chuyển hướng đến trang chủ hoặc trang khác
                return RedirectToAction("Index", "TrangChu");  // Ví dụ: Trang chủ
            }
            return View();
        }

        [HttpGet]
        public IActionResult DangKy()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (User.Identity.IsAuthenticated)
            {
                // Nếu đã đăng nhập rồi, chuyển hướng đến trang chủ hoặc trang khác
                return RedirectToAction("Index", "TrangChu");  // Ví dụ: Trang chủ
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKiViewModel user)
        {

            if (ModelState.IsValid)
            {
                var dbUser = await _dataContext.TaiKhoans
                   .FirstOrDefaultAsync(u => u.Email == user.Email);
                if (dbUser == null)
                {
                    if (user.Password == user.RePassword)
                    {
                        // Mã hóa mật khẩu
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                        TaiKhoanModel moi = new TaiKhoanModel
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            Password = hashedPassword,
                            ChucVu = "user"
                        };

                        await _dataContext.TaiKhoans.AddAsync(moi); // Thêm bản ghi mới
                        await _dataContext.SaveChangesAsync();

                        var GioHang = new GioHangModel
                        {
                            FK_TaiKhoan = moi.PK_TaiKhoan
                        };
                        await _dataContext.GioHangs.AddAsync(GioHang); // Thêm Giỏ Hàng mới
                        await _dataContext.SaveChangesAsync();

                        // Chuyển hướng về trang đăng nhập sau khi đăng ký thành công
                        TempData["SuccessMessage"] = "Đăng ký thành công!";
                        return RedirectToAction("Login", "TaiKhoan");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Mật khẩu không khớp!";
                        ModelState.AddModelError(string.Empty, "Mật khẩu không khớp!");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Email đã tồn tại!";
                    ModelState.AddModelError(string.Empty, "Email đã tồn tại!");
                }
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(DangNhapViewModel user)
        {

            if (ModelState.IsValid)
            {
                // Tìm người dùng dựa trên email
                var dbUser = await _dataContext.TaiKhoans.FirstOrDefaultAsync(u => u.Email == user.Email);

                if (dbUser != null && BCrypt.Net.BCrypt.Verify(user.Password, dbUser.Password))
                {
                    // Tạo các Claims cho người dùng
                    var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, dbUser.UserName),
                                new Claim(ClaimTypes.Email, dbUser.Email),
                                new Claim(ClaimTypes.Role, dbUser.ChucVu)
                            };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // Đăng nhập và lưu thông tin vào cookie
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // Kiểm tra chức vụ và chuyển hướng
                    TempData["SuccessMessage"] = "Đăng nhập thành công!";
                    // Kiểm tra chức vụ và chuyển hướng
                    if (dbUser.ChucVu == "admin")
                    {
                        return RedirectToAction("Index", "ThongKe", new { area = "Admin" });
                    }
                    else if (dbUser.ChucVu == "user")
                    {
                        return RedirectToAction("Index", "TrangChu");
                    }
                }

                // Nếu thông tin đăng nhập sai
                TempData["ErrorMessage"] = "Sai email hoặc mật khẩu!";
                ModelState.AddModelError(string.Empty, "Sai email hoặc mật khẩu");
            }

            return View(user);
        }

        // Đăng xuất
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công!";
            return RedirectToAction("Login", "TaiKhoan");
        }

        //Quên Mật Khẩu
        public IActionResult QuenMatKhau()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> QuenMatKhau(string email)
        {
            var user = _dataContext.TaiKhoans.FirstOrDefault(x => x.Email == email);
            if (user == null)
            {
                ViewBag.Message = "Email không tồn tại.";
                return View();
            }

            // Tạo token reset
            var token = Guid.NewGuid().ToString();
            user.ResetToken = token;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);

            _dataContext.Update(user);
            await _dataContext.SaveChangesAsync();

            // Tạo đường link reset
            var resetLink = Url.Action("DatLaiMatKhau", "TaiKhoan", new { token = token }, Request.Scheme);
            var body = $"Click vào link sau để đặt lại mật khẩu: <a href='{resetLink}'>Đặt lại mật khẩu</a>";

            // Gửi email
            await _emailSender.SendEmailAsync(user.Email, "Khôi phục mật khẩu", body);

            ViewBag.Message = "Đã gửi liên kết đặt lại mật khẩu đến email của bạn.";
            return View();
        }

        public IActionResult DatLaiMatKhau(string token)
        {
            var user = _dataContext.TaiKhoans.FirstOrDefault(x => x.ResetToken == token && x.ResetTokenExpiry > DateTime.UtcNow);
            if (user == null)
                return NotFound("Liên kết không hợp lệ hoặc đã hết hạn.");

            return View(model: token);
        }

        [HttpPost]
        public async Task<IActionResult> DatLaiMatKhau(string token, string newPassword)
        {
            var user = _dataContext.TaiKhoans.FirstOrDefault(x => x.ResetToken == token && x.ResetTokenExpiry > DateTime.UtcNow);
            if (user == null)
                return NotFound("Liên kết không hợp lệ hoặc đã hết hạn.");

            // Mã hóa mật khẩu mới
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // Xóa token
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            _dataContext.Update(user);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction("Login", "TaiKhoan");
        }

        public IActionResult ThongTinTaiKhoan()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var tk = _dataContext.TaiKhoans.FirstOrDefault(x => x.Email == email);
            return View(tk);
        }

        [HttpPost]
        public IActionResult CapNhatTaiKhoan(TaiKhoanModel model)
        {
            var taiKhoan = _dataContext.TaiKhoans.FirstOrDefault(x => x.PK_TaiKhoan == model.PK_TaiKhoan);
            if (taiKhoan != null)
            {
                taiKhoan.UserName = model.UserName;
                _dataContext.SaveChanges();
            }
            TempData["SuccessMessage"] = "Bạn đã cập nhật thông tin thành công!";
            return RedirectToAction("ThongTinTaiKhoan");
        }

        [HttpPost]
        public IActionResult DoiMatKhau(int PK_TaiKhoan, string NewPassword, string ConfirmPassword)
        {
            var taiKhoan = _dataContext.TaiKhoans.FirstOrDefault(x => x.PK_TaiKhoan == PK_TaiKhoan);
            if (taiKhoan != null && !string.IsNullOrEmpty(NewPassword) && NewPassword == ConfirmPassword)
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(NewPassword);
                taiKhoan.Password = hashedPassword; // Hash nếu cần
                _dataContext.SaveChanges();
                TempData["SuccessMessage"] = "Bạn đã đổi mk thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Đổi mật khẩu không thành công!";
            }

            return RedirectToAction("ThongTinTaiKhoan");
        }

    }
}
