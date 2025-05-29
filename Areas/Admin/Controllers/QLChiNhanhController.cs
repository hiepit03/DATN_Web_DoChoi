using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_DoChoi.Models;
using Web_DoChoi.Repository;

namespace Web_DoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]  // Chỉ cho phép người dùng có role "admin" truy cập
    public class QLChiNhanhController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public QLChiNhanhController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {   
            var cn = _dataContext.ChiNhanhs.ToList();
            return View(cn);
        }

        //Xóa
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {

            var item = await _dataContext.ChiNhanhs.FindAsync(id);
            if (item == null)
            {
                TempData["ErrorMessage"] = "Chi Nhánh không tồn tại.";
                return NotFound();
            }

            _dataContext.ChiNhanhs.Remove(item);
            await _dataContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa chi nhánh thành công!";

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
        public async Task<IActionResult> Create(ChiNhanhModel cn)
        {
            if (ModelState.IsValid)
            {
                //them du lieu
                var check = await _dataContext.ChiNhanhs.FirstOrDefaultAsync(p => p.PK_ChiNhanh == cn.PK_ChiNhanh || p.TenChiNhanh == cn.TenChiNhanh);
                if (check != null)
                {
                    TempData["ErrorMessage"] = "Đã tồn tại Chi Nhánh";
                    ModelState.AddModelError("", "Chi Nhánh đã có trong database");
                    return View(cn);
                }

                _dataContext.ChiNhanhs.Add(cn);
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

        

    }
}
