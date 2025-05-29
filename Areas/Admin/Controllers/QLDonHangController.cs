using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_DoChoi.Models;
using Web_DoChoi.Repository;
using Web_DoChoi.ViewModel;

namespace Web_DoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]  // Chỉ cho phép người dùng có role "admin" truy cập
    public class QLDonHangController : Controller
    {

        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public QLDonHangController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {   
            var dh = _dataContext.DonHangs.ToList();
            return View(dh);
        }

        public async Task<int> TinhLoiNhuanDonHang(int donHangId)
        {
            var chiTietDonHangs = await _dataContext.ChiTietDonHangs
                .Include(ct => ct.DoChoi)
                .Where(ct => ct.FK_DonHang == donHangId)
                .ToListAsync();

            int loiNhuan = chiTietDonHangs.Sum(ct =>
                (ct.DoChoi.GiaBan - ct.DoChoi.GiaNhapHang) * ct.SoLuongSP
            );

            return loiNhuan;
        }

        public async Task CapNhatLoiNhuanChoDonHang(int donHangId)
        {
            var donHang = await _dataContext.DonHangs.FindAsync(donHangId);
            if (donHang == null) return;

            donHang.LoiNhuan = await TinhLoiNhuanDonHang(donHangId);
            await _dataContext.SaveChangesAsync();
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThai(int id, string trangThai)
        {
            var donHang = _dataContext.DonHangs.FirstOrDefault(d => d.PK_DonHang == id);
            if (donHang == null)
            {
                return NotFound();
            }

            donHang.TrangThaiDonHang = trangThai;
            _dataContext.SaveChanges();

            if(donHang.TrangThaiDonHang == "Đã Hủy")
            {

                // Lấy danh sách chi tiết đơn hàng
                var chiTietDonHangs = _dataContext.ChiTietDonHangs
                    .Where(ct => ct.FK_DonHang == donHang.PK_DonHang)
                    .ToList();

                foreach (var ct in chiTietDonHangs)
                {
                    var doChoi = _dataContext.DoChois.FirstOrDefault(dc => dc.PK_DoChoi == ct.FK_DoChoi);
                    if (doChoi != null)
                    {
                        doChoi.SoLuongDaBan -= ct.SoLuongSP;
                        doChoi.SoLuongConLai += ct.SoLuongSP;
                    }
                }
            }

            if (donHang.TrangThaiDonHang == "Giao Hàng Thành Công")
            {
                await CapNhatLoiNhuanChoDonHang(id);
                if(donHang.PhuongThucThanhToan == "COD")
                {
                    donHang.TrangThaiThanhToan = true;
                    await _dataContext.SaveChangesAsync();
                }
            }

            return Ok();
        }


        [HttpGet]
        public IActionResult XemChiTiet(int id)
        {
            var donhang = _dataContext.DonHangs.FirstOrDefault(x => x.PK_DonHang == id);
            if (donhang == null)
            {
                return RedirectToAction("Error", "Home", new { statuscode = 404 });
            }

            var chitietdonhang = _dataContext.ChiTietDonHangs
                                    .Include(ct => ct.DoChoi)
                                    .Where(x => x.FK_DonHang == id)
                                    .ToList();

            var viewModel = new DonHangViewModel
            {
                DonHang = donhang,
                ChiTietDonHangs = chitietdonhang.Select(ct => new ChiTietDonHangViewModel
                {
                    SoLuongSP = ct.SoLuongSP,
                    MaDoChoi = ct.DoChoi.PK_DoChoi,
                    TenDoChoi = ct.DoChoi.TenDoChoi,
                    Gia = ct.DoChoi.GiaBan,
                    HinhAnh = ct.DoChoi.AnhDoChoi
                }).ToList()
            };

            return View(viewModel);
        }

    }
}
