using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_DoChoi.Models;
using Web_DoChoi.Repository;

namespace Web_DoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]  // Chỉ cho phép người dùng có role "admin" truy cập
    public class UserController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {   
            var user = _dataContext.TaiKhoans.ToList();
            return View(user);
        }

        //Xóa
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {

            var item = await _dataContext.TaiKhoans.FindAsync(id);
            if (item == null)
            {
                TempData["ErrorMessage"] = "Tài Khoản không tồn tại.";
                return NotFound();
            }

            _dataContext.TaiKhoans.Remove(item);
            await _dataContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa Tài Khoản thành công!";

            return Json(new { success = true });
        }

        //thêm
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaiKhoanModel tk)
        {
            if (ModelState.IsValid)
            {
                //them du lieu
                var check = await _dataContext.TaiKhoans.FirstOrDefaultAsync(p => p.PK_TaiKhoan == tk.PK_TaiKhoan || p.Email == tk.Email);
                if (check != null)
                {
                    TempData["ErrorMessage"] = "Đã tồn tại Email";
                    ModelState.AddModelError("", "Email đã có trong database");
                    return View(tk);
                }

                // Mã hóa mật khẩu
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(tk.Password);

                TaiKhoanModel moi = new TaiKhoanModel
                {
                    UserName = tk.UserName,
                    Email = tk.Email,
                    Password = hashedPassword,
                    ChucVu = tk.ChucVu
                };

                _dataContext.TaiKhoans.Add(moi);
                await _dataContext.SaveChangesAsync();

                var GioHang = new GioHangModel
                {
                    FK_TaiKhoan = moi.PK_TaiKhoan
                };
                await _dataContext.GioHangs.AddAsync(GioHang); // Thêm Giỏ Hàng mới
                await _dataContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Model có 1 số lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string err = string.Join("\n", errors);
                return BadRequest(err);

            }
        }

        //Sửa
        public async Task<IActionResult> Edit(int Id)
        {
            var nsx = await _dataContext.TaiKhoans.FirstOrDefaultAsync(x => x.PK_TaiKhoan == Id);
            return View(nsx);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TaiKhoanModel tk)
        {

            if (ModelState.IsValid)
            {

                // Mã hóa mật khẩu
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(tk.Password);

                tk.Password = hashedPassword;

                // Cập nhật phim trong cơ sở dữ liệu
                _dataContext.TaiKhoans.Update(tk);
                try
                {
                    await _dataContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi lưu thông tin tk: {ex.Message}";
                    return BadRequest($"Lỗi khi lưu thông tin tk: {ex.Message}");
                }

                TempData["SuccessMessage"] = "Cập nhật thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Model có 1 số lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string err = string.Join("\n", errors);
                return BadRequest(err);
            }

        }

    }
}
