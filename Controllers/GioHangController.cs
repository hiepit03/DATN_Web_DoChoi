using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_DoChoi.Models;
using Web_DoChoi.Repository;
using Web_DoChoi.ViewModel;

namespace Web_DoChoi.Controllers
{
    public class GioHangController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<GioHangController> _logger;

        public GioHangController(ILogger<GioHangController> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }
        public async Task<IActionResult> Index()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var tk = await _dataContext.TaiKhoans.FirstOrDefaultAsync(x => x.Email == email);
            var giohang = await _dataContext.GioHangs.FirstOrDefaultAsync(x => x.FK_TaiKhoan == tk.PK_TaiKhoan);

            var spgh = await _dataContext.ChiTietGioHangs
                .Where(x => x.FK_GioHang == giohang.PK_GioHang)
                .Include(x => x.DoChoi)
                .ToListAsync();

            var viewModelList = spgh.Select(p => new SanPhamGioHangViewModel
            {
                ID_GioHang = giohang.PK_GioHang,
                ChiTietGioHangId = p.PK_ChiTietGioHang,
                HinhAnh = p.DoChoi.AnhDoChoi,
                TenDoChoi = p.DoChoi.TenDoChoi,
                GiaDoChoi = p.DoChoi.GiaBan,
                SoLuongSP = p.SoLuongSP
            }).ToList();

            return View(viewModelList);
        }

        [HttpGet]
        public async Task<IActionResult> GioHangDropdown()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var tk = await _dataContext.TaiKhoans.FirstOrDefaultAsync(x => x.Email == email);
            var giohang = await _dataContext.GioHangs.FirstOrDefaultAsync(x => x.FK_TaiKhoan == tk.PK_TaiKhoan);

            var spgh = await _dataContext.ChiTietGioHangs
                .Where(x => x.FK_GioHang == giohang.PK_GioHang)
                .Include(x => x.DoChoi)
                .ToListAsync();

            var viewModelList = spgh.Select(p => new SanPhamGioHangViewModel
            {
                ID_GioHang = giohang.PK_GioHang,
                ChiTietGioHangId = p.PK_ChiTietGioHang,
                HinhAnh = p.DoChoi.AnhDoChoi,
                TenDoChoi = p.DoChoi.TenDoChoi,
                GiaDoChoi = p.DoChoi.GiaBan,
                SoLuongSP = p.SoLuongSP
            }).ToList();

            return ViewComponent("GioHangDropdown", viewModelList);
        }


        [HttpGet]
        public async Task<IActionResult> GioHangSoLuong()
        {
            var email = HttpContext.User.FindFirstValue(ClaimTypes.Email);

            var tk = await _dataContext.TaiKhoans.FirstOrDefaultAsync(p => p.Email == email);

            var gioHang = await _dataContext.GioHangs.FirstOrDefaultAsync(g => g.FK_TaiKhoan == tk.PK_TaiKhoan);

            var ctgh = await _dataContext.ChiTietGioHangs.Where(x => x.FK_GioHang == gioHang.PK_GioHang).ToListAsync();

            int tongSoLuong = ctgh.Sum(x => x.SoLuongSP);

            return ViewComponent("SoSanPhamGioHang", tongSoLuong);
        }

        [HttpPost]
        public IActionResult ThemGioHang(int iddochoi, int soluong)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thêm giỏ hàng." });
            }
            else
            {
                // Lấy email từ Claims của người dùng
                var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                var tk = _dataContext.TaiKhoans.FirstOrDefault(x => x.Email == email);
                var giohang = _dataContext.GioHangs.FirstOrDefault(x => x.FK_TaiKhoan == tk.PK_TaiKhoan);

                // Kiểm tra nếu sản phẩm đã có trong giỏ hàng
                var existingItem = _dataContext.ChiTietGioHangs
                    .FirstOrDefault(x => x.FK_DoChoi == iddochoi && x.FK_GioHang == giohang.PK_GioHang);

                var dochoi = _dataContext.DoChois.FirstOrDefault(x => x.PK_DoChoi == iddochoi);

                if (existingItem != null)
                {
                    
                    // Nếu sản phẩm đã có trong giỏ hàng, cập nhật số lượng
                    existingItem.SoLuongSP += soluong;  // Cộng thêm số lượng

                    if (existingItem.SoLuongSP > dochoi.SoLuongConLai)
                    {
                        return Json(new { success = false, message = "Số Lượng Sản Phẩm Thêm Vượt Quá Tồn Kho: " + dochoi.SoLuongConLai +"SP" });
                    }

                    _dataContext.ChiTietGioHangs.Update(existingItem);  // Cập nhật bản ghi
                    _dataContext.SaveChanges();
                    return Json(new { success = true, message = "Thêm Giỏ Hàng Thành Công." });
                }

                if (soluong > dochoi.SoLuongConLai)
                {
                    return Json(new { success = false, message = "Số Lượng Sản Phẩm Thêm Vượt Quá Tồn Kho: " + dochoi.SoLuongConLai + "SP" });
                }

                var ctgh = new ChiTietGioHangModel
                {
                    SoLuongSP = soluong,
                    FK_DoChoi = iddochoi,
                    FK_GioHang = giohang.PK_GioHang
                };

                _dataContext.ChiTietGioHangs.Add(ctgh);
                _dataContext.SaveChanges();

                return Json(new { success = true, message = "Thêm Giỏ Hàng Thành Công" });
            }
        }

        [HttpPost]
        public IActionResult XoaGioHang(int id)
        {
            // Xử lý logic xóa sản phẩm trong giỏ hàng
            var ctgh = _dataContext.ChiTietGioHangs.FirstOrDefault(g => g.PK_ChiTietGioHang == id);

            if (ctgh != null)
            {
                // Xóa sản phẩm khỏi giỏ hàng
                _dataContext.ChiTietGioHangs.Remove(ctgh);
                _dataContext.SaveChanges();

                // Trả về kết quả thành công
                return Json(new { success = true, message = "Xóa Thành Công" });
            }

            // Nếu không tìm thấy sản phẩm, trả về kết quả lỗi
            return Json(new { success = false, message = "có lỗi" });
        }

        [HttpPost]
        public async Task<IActionResult> SuaSoLuongDoChoi(int idgiohang, int soluong)
        {
            var ctgh = await _dataContext.ChiTietGioHangs.FirstOrDefaultAsync(x => x.PK_ChiTietGioHang == idgiohang);
            var dochoi = await _dataContext.DoChois.FirstOrDefaultAsync(x => x.PK_DoChoi == ctgh.FK_DoChoi);
            if (soluong > dochoi.SoLuongConLai)
            {
                TempData["ThongBao"] = "Số lượng vượt quá sản phẩm tồn kho!" + dochoi.SoLuongConLai +"SP!";

            }
            else
            {
                ctgh.SoLuongSP = soluong;
                await _dataContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> XoaDoChoi(int idgiohang)
        {
            var ctgh = await _dataContext.ChiTietGioHangs.FirstOrDefaultAsync(x => x.PK_ChiTietGioHang == idgiohang);
            _dataContext.ChiTietGioHangs.Remove(ctgh);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
