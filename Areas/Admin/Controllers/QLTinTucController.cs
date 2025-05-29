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
    public class QLTinTucController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public QLTinTucController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {   
            var tintuc = _dataContext.TinTucs.ToList();
            return View(tintuc);
        }

        //Xóa
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {

            var item = await _dataContext.TinTucs.FindAsync(id);
            if (item == null)
            {
                TempData["ErrorMessage"] = "Tin Tức không tồn tại.";
                return NotFound();
            }

            //xóa ảnh nếu có
            if (!string.IsNullOrEmpty(item.HinhAnh))
            {
                string uploadsdir = Path.Combine(_webHostEnvironment.WebRootPath, "media/tintuc");
                string filepath = Path.Combine(uploadsdir, item.HinhAnh);

                // kiểm tra xem tệp có tồn tại không và xóa nó
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
            }

            _dataContext.TinTucs.Remove(item);
            await _dataContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa tin tức thành công!";

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
        public async Task<IActionResult> Create(TinTucModel TinTuc)
        {
            if (ModelState.IsValid)
            {
                //them du lieu
                var check = await _dataContext.TinTucs.FirstOrDefaultAsync(p => p.PK_TinTuc == TinTuc.PK_TinTuc);
                if (check != null)
                {
                    TempData["ErrorMessage"] = "Đã tồn tại Tin Tức";
                    ModelState.AddModelError("", "Tin Tức đã có trong database");
                    return View(TinTuc);
                }

                if (TinTuc.AnhUpLoad != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/tintuc");
                    string imgName = Guid.NewGuid().ToString() + "_" + TinTuc.AnhUpLoad.FileName;
                    string filePath = Path.Combine(uploadsDir, imgName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await TinTuc.AnhUpLoad.CopyToAsync(fs);
                    fs.Close();

                    TinTuc.HinhAnh = imgName;
                }

                //// Gán lại navigation property nếu cần dùng đến
                //DoChoi.DanhMuc = _dataContext.DanhMucs.Find(DoChoi.FK_DanhMuc);
                //DoChoi.NhaSanXuat = _dataContext.NhaSanXuats.Find(DoChoi.FK_NhaSanXuat);

                _dataContext.TinTucs.Add(TinTuc);
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
