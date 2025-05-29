using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_DoChoi.Models;
using Web_DoChoi.Repository;

namespace Web_DoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]  // Chỉ cho phép người dùng có role "admin" truy cập
    public class NhaSanXuatController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NhaSanXuatController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var nsx = _dataContext.NhaSanXuats.ToList();
            return View(nsx);
        }

        //Xóa
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {

            var item = await _dataContext.NhaSanXuats.FindAsync(id);
            if (item == null)
            {
                TempData["ErrorMessage"] = "Nhà Sản Xuất không tồn tại.";
                return NotFound();
            }

            _dataContext.NhaSanXuats.Remove(item);
            await _dataContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa nhà sản xuất thành công!";

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
        public async Task<IActionResult> Create(NhaSanXuatModel nsx)
        {
            if (ModelState.IsValid)
            {
                //them du lieu
                var check = await _dataContext.NhaSanXuats.FirstOrDefaultAsync(p => p.PK_NSX == nsx.PK_NSX || p.TenNSX == nsx.TenNSX);
                if (check != null)
                {
                    TempData["ErrorMessage"] = "Đã tồn tại Nhà Sản Xuất";
                    ModelState.AddModelError("", "Nhà Sản Xuất đã có trong database");
                    return View(nsx);
                }

                _dataContext.NhaSanXuats.Add(nsx);
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
            var nsx = await _dataContext.NhaSanXuats.FirstOrDefaultAsync(x => x.PK_NSX == Id);
            return View(nsx);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NhaSanXuatModel nsx)
        {

            if (ModelState.IsValid)
            {

                // Cập nhật phim trong cơ sở dữ liệu
                _dataContext.NhaSanXuats.Update(nsx);
                try
                {
                    await _dataContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi lưu thông tin nsx: {ex.Message}";
                    return BadRequest($"Lỗi khi lưu thông tin nsx: {ex.Message}");
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
