using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_DoChoi.Models;
using Web_DoChoi.Repository;

namespace Web_DoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]  // Chỉ cho phép người dùng có role "admin" truy cập
    public class SlideController : Controller
    {

        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SlideController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var slide = _dataContext.Slides.ToList();
            return View(slide);
        }

        //Xóa
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {

            var item = await _dataContext.Slides.FindAsync(id);
            if (item == null)
            {
                TempData["ErrorMessage"] = "Slide không tồn tại.";
                return NotFound();
            }

            //xóa ảnh nếu có
            if (!string.IsNullOrEmpty(item.HinhAnh))
            {
                string uploadsdir = Path.Combine(_webHostEnvironment.WebRootPath, "media/slide");
                string filepath = Path.Combine(uploadsdir, item.HinhAnh);

                // kiểm tra xem tệp có tồn tại không và xóa nó
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
            }

            _dataContext.Slides.Remove(item);
            await _dataContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa Slide thành công!";

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
        public async Task<IActionResult> Create(SlideModel sl)
        {
            if (ModelState.IsValid)
            {
                //them du lieu
                var check = await _dataContext.Slides.FirstOrDefaultAsync(p => p.PK_Slide == sl.PK_Slide);
                if (check != null)
                {
                    TempData["ErrorMessage"] = "Đã tồn tại Slide";
                    ModelState.AddModelError("", "Phim đã có trong Slide");
                    return View(sl);
                }

                if (sl.AnhUpLoad != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/slide");
                    string imgName = Guid.NewGuid().ToString() + "_" + sl.AnhUpLoad.FileName;
                    string filePath = Path.Combine(uploadsDir, imgName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await sl.AnhUpLoad.CopyToAsync(fs);
                    fs.Close();

                    sl.HinhAnh = imgName;
                }

                _dataContext.Slides.Add(sl);
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
