using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_DoChoi.Models;
using Web_DoChoi.Repository;

namespace Web_DoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]  // Chỉ cho phép người dùng có role "admin" truy cập
    public class DanhMucController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DanhMucController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {   
            var dm = _dataContext.DanhMucs.ToList();
            return View(dm);
        }

        //Xóa
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {

            var item = await _dataContext.DanhMucs.FindAsync(id);
            if (item == null)
            {
                TempData["ErrorMessage"] = "Danh Mục không tồn tại.";
                return NotFound();
            }

            _dataContext.DanhMucs.Remove(item);
            await _dataContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa Danh Mục thành công!";

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
        public async Task<IActionResult> Create(DanhMucModel dm)
        {
            if (ModelState.IsValid)
            {
                //them du lieu
                var check = await _dataContext.DanhMucs.FirstOrDefaultAsync(p => p.PK_DanhMuc == dm.PK_DanhMuc || p.TenDanhMuc == dm.TenDanhMuc);
                if (check != null)
                {
                    TempData["ErrorMessage"] = "Đã tồn tại Danh Mục";
                    ModelState.AddModelError("", "Danh Mục đã có trong database");
                    return View(dm);
                }

                _dataContext.DanhMucs.Add(dm);
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
            var nsx = await _dataContext.DanhMucs.FirstOrDefaultAsync(x => x.PK_DanhMuc == Id);
            return View(nsx);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DanhMucModel dm)
        {

            if (ModelState.IsValid)
            {

                // Cập nhật phim trong cơ sở dữ liệu
                _dataContext.DanhMucs.Update(dm);
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
