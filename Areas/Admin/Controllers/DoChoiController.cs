using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_DoChoi.Models;
using Web_DoChoi.Repository;

namespace Web_DoChoi.Area.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]  // Chỉ cho phép người dùng có role "admin" truy cập
    public class DoChoiController : Controller
    {

        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DoChoiController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var dochoi = _dataContext.DoChois.ToList();
            return View(dochoi);
        }

        //Xóa
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {

            var item = await _dataContext.DoChois.FindAsync(id);
            if (item == null)
            {
                TempData["ErrorMessage"] = "Đồ Chơi không tồn tại.";
                return NotFound();
            }

            //xóa ảnh nếu có
            if (!string.IsNullOrEmpty(item.AnhDoChoi))
            {
                string uploadsdir = Path.Combine(_webHostEnvironment.WebRootPath, "media/dochoi");
                string filepath = Path.Combine(uploadsdir, item.AnhDoChoi);

                // kiểm tra xem tệp có tồn tại không và xóa nó
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
            }

            _dataContext.DoChois.Remove(item);
            await _dataContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa đồ chơi thành công!";

            return Json(new { success = true });
        }

        //thêm
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.NhaSanXuat = new SelectList(_dataContext.NhaSanXuats, "PK_NSX", "TenNSX");
            ViewBag.DanhMuc = new SelectList(_dataContext.DanhMucs, "PK_DanhMuc", "TenDanhMuc");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoChoiModel DoChoi)
        {
            if (ModelState.IsValid)
            {
                //them du lieu
                var check = await _dataContext.DoChois.FirstOrDefaultAsync(p => p.PK_DoChoi == DoChoi.PK_DoChoi);
                if (check != null)
                {
                    TempData["ErrorMessage"] = "Đã tồn tại phim";
                    ModelState.AddModelError("", "Phim đã có trong database");
                    return View(DoChoi);
                }

                if (DoChoi.AnhUpLoad != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/dochoi");
                    string imgName = Guid.NewGuid().ToString() + "_" + DoChoi.AnhUpLoad.FileName;
                    string filePath = Path.Combine(uploadsDir, imgName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await DoChoi.AnhUpLoad.CopyToAsync(fs);
                    fs.Close();

                    DoChoi.AnhDoChoi = imgName;
                }

                //// Gán lại navigation property nếu cần dùng đến
                //DoChoi.DanhMuc = _dataContext.DanhMucs.Find(DoChoi.FK_DanhMuc);
                //DoChoi.NhaSanXuat = _dataContext.NhaSanXuats.Find(DoChoi.FK_NhaSanXuat);

                _dataContext.DoChois.Add(DoChoi);
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
            var dochoi = await _dataContext.DoChois.FirstOrDefaultAsync(x => x.PK_DoChoi == Id);
            ViewBag.NhaSanXuat = new SelectList(_dataContext.NhaSanXuats, "PK_NSX", "TenNSX");
            ViewBag.DanhMuc = new SelectList(_dataContext.DanhMucs, "PK_DanhMuc", "TenDanhMuc");
            return View(dochoi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DoChoiModel DoChoi)
        {
            int Id = DoChoi.PK_DoChoi;

            if (ModelState.IsValid)
            {
                // Đảm bảo rằng Phim.PK_Phim trùng với Id truyền vào
                if (DoChoi.PK_DoChoi != Id)
                {
                    TempData["ErrorMessage"] = "Mã Đồ Chơi không hợp lệ.";
                    return BadRequest("Mã Đồ Chơi không hợp lệ.");
                }

                // Lấy thông tin phim cũ từ cơ sở dữ liệu mà không theo dõi
                var existingdochoi = await _dataContext.DoChois.AsNoTracking().FirstOrDefaultAsync(p => p.PK_DoChoi == Id);
                if (existingdochoi == null)
                {
                    TempData["ErrorMessage"] = "Đồ Chơi không tồn tại.";
                    return NotFound();
                }

                // Xử lý ảnh mới nếu có
                if (DoChoi.AnhUpLoad != null)
                {
                    // Đường dẫn đến thư mục chứa ảnh
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/dochoi");
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }

                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(existingdochoi.AnhDoChoi))
                    {
                        string oldFilePath = Path.Combine(uploadsDir, existingdochoi.AnhDoChoi);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            try
                            {
                                System.IO.File.Delete(oldFilePath); // Xóa ảnh cũ
                            }
                            catch (Exception ex)
                            {
                                TempData["ErrorMessage"] = $"Lỗi khi xóa ảnh cũ: {ex.Message}";
                                return BadRequest($"Lỗi khi xóa ảnh cũ: {ex.Message}");
                            }
                        }
                    }

                    // Lưu ảnh mới
                    string imgName = Guid.NewGuid().ToString() + "_" + DoChoi.AnhUpLoad.FileName;
                    string filePath = Path.Combine(uploadsDir, imgName);
                    try
                    {
                        using (FileStream fs = new FileStream(filePath, FileMode.Create))
                        {
                            await DoChoi.AnhUpLoad.CopyToAsync(fs);
                        }
                        DoChoi.AnhDoChoi = imgName; // Cập nhật tên ảnh mới
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = $"Lỗi khi tải ảnh mới lên: {ex.Message}";
                        return BadRequest($"Lỗi khi tải ảnh mới lên: {ex.Message}");
                    }
                }
                else
                {
                    // Nếu không có ảnh mới, giữ nguyên ảnh cũ
                    DoChoi.AnhDoChoi = existingdochoi.AnhDoChoi;
                }

                // Cập nhật phim trong cơ sở dữ liệu
                _dataContext.DoChois.Update(DoChoi);
                try
                {
                    await _dataContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi lưu thông tin Đồ chơi: {ex.Message}";
                    return BadRequest($"Lỗi khi lưu thông tin Đồ Chơi: {ex.Message}");
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
