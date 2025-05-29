using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using System.Security.Claims;
using Web_DoChoi.Repository;
using Web_DoChoi.ViewModel;
using Rotativa.AspNetCore;
using Web_DoChoi.Models;

namespace Web_DoChoi.Controllers
{
    public class XemDonHangController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<XemDonHangController> _logger;

        public XemDonHangController(ILogger<XemDonHangController> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }
        public async Task<IActionResult> Index()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var tk = await _dataContext.TaiKhoans.FirstOrDefaultAsync(x => x.Email == email);
            var donhang = await _dataContext.DonHangs.Include(x =>x.TaiKhoan).Where(x => x.FK_TaiKhoan == tk.PK_TaiKhoan).ToListAsync();

            return View(donhang);
        }

        [HttpPost]
        public IActionResult HuyDonHang(int id, string trangThai)
        {
            var donHang = _dataContext.DonHangs.FirstOrDefault(d => d.PK_DonHang == id);
            if (donHang == null)
                return NotFound();


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

            donHang.TrangThaiDonHang = trangThai;
            _dataContext.SaveChanges();

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

        //Xuất File PDF-------------------------------------------------------------
        [HttpGet]
        public IActionResult XuatPDF(int id)
        {
            var donhang = _dataContext.DonHangs.FirstOrDefault(x => x.PK_DonHang == id);
            if (donhang == null) return NotFound();

            var chitiet = _dataContext.ChiTietDonHangs
                            .Include(x => x.DoChoi)
                            .Where(x => x.FK_DonHang == id)
                            .ToList();

           
            var viewModel = new DonHangViewModel
            {
                DonHang = donhang,
                ChiTietDonHangs = chitiet.Select(ct => new ChiTietDonHangViewModel
                {
                    SoLuongSP = ct.SoLuongSP,
                    MaDoChoi = ct.DoChoi.PK_DoChoi,
                    TenDoChoi = ct.DoChoi.TenDoChoi,
                    Gia = ct.DoChoi.GiaBan,
                    HinhAnh = ct.DoChoi.AnhDoChoi
                }).ToList()
            };

            return new ViewAsPdf("XuatPDF", viewModel)
            {
                FileName = $"DonHang_{id}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }

        [HttpPost]
        public IActionResult GuiDanhGia(int MaDoChoi, int SoSao, string NoiDung)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var tk = _dataContext.TaiKhoans.FirstOrDefault(x => x.Email == email);

            // Lưu vào database ở đây
            var danhGia = new DanhGiaModel
            {
                SoSaoDanhGia = SoSao,
                LoiDanhGia = NoiDung,
                FK_DoChoi = MaDoChoi,
                FK_TaiKhoan = tk.PK_TaiKhoan,
                ThoiGianThem = DateTime.Now 
            };

            _dataContext.DanhGias.Add(danhGia);
            _dataContext.SaveChanges();
           

            return Ok();
        }

    }
}
