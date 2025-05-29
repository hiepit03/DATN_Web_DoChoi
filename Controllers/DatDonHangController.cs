using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_DoChoi.Models;
using Web_DoChoi.Repository;
using Web_DoChoi.Services.Momo;
using Web_DoChoi.ViewModel;

namespace Web_DoChoi.Controllers
{
    public class DatDonHangController : Controller
    {

        private readonly DataContext _dataContext;
        private readonly ILogger<DatDonHangController> _logger;

        public DatDonHangController(ILogger<DatDonHangController> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public IActionResult Index(string ghiChu)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var tk = _dataContext.TaiKhoans.FirstOrDefault(x => x.Email == email);
            var dcs = _dataContext.DiaChis.Where(x=>x.FK_TaiKhoan == tk.PK_TaiKhoan).ToList();

            var ghang = _dataContext.GioHangs.FirstOrDefault(x => x.FK_TaiKhoan == tk.PK_TaiKhoan);
            var ctgh = _dataContext.ChiTietGioHangs.Where(x => x.FK_GioHang == ghang.PK_GioHang).ToList();

            if(ctgh.Count == 0)
            {
                TempData["ThongBao"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm trước khi đặt hàng.";
                return RedirectToAction("Index", "GioHang"); // hoặc về trang chủ/sp tuỳ bạn
            }

            var now = DateTime.Now;
            var mkm = _dataContext.MaKhuyenMais
                .Where(x => x.ThoiGianBatDau <= now && now <= x.ThoiGianKetThuc)
                .ToList();
            DatDonHangViewModel viewmodel = new DatDonHangViewModel
            {
                HoTen = username,
                email = email,
                DiaChis = dcs,
                MaKhuyenMais = mkm
            };

            ViewBag.ghichu = ghiChu;

            return View(viewmodel);
        }

        [HttpPost]
        public IActionResult KiemTraMa(string ma, int tongtien)
        {

            // Nếu đã áp dụng mã trong session → không cho áp dụng lại
            if (HttpContext.Session.GetString("DaApDungMa") == "true")
            {
                return Json(new
                {
                    hopLe = false,
                    thongBao = "Bạn đã áp dụng mã cho đơn hàng rồi."
                });
            }

            if (string.IsNullOrWhiteSpace(ma))
            {
                return Json(new
                {
                    hopLe = false,
                    thongBao = "Vui lòng nhập mã khuyến mãi."
                });
            }

            var now = DateTime.Now;
            var mkm = _dataContext.MaKhuyenMais.FirstOrDefault(x => x.TenMaGiamGia == ma && x.ThoiGianBatDau <= now && now <= x.ThoiGianKetThuc);
            if (mkm == null)
            {
                return Json(new
                {
                    hopLe = false,
                    thongBao = "Mã khuyến mãi không tồn tại!"
                });
            }

            if (tongtien < mkm.GiaTienApDung) // thường so sánh "<" thay vì "<="
            {
                return Json(new
                {
                    hopLe = false,
                    thongBao = $"Đơn hàng cần tối thiểu {mkm.GiaTienApDung:N0}đ để áp dụng mã."
                });
            }

            // ✅ Ghi nhớ mã đã áp dụng trong session
            HttpContext.Session.SetString("DaApDungMa", "true");

            int giamGia = mkm.GiaTienGiam;
            int tongtienmoi = tongtien - giamGia;

            return Json(new
            {
                hopLe = true,
                giamGia = giamGia,
                tongTienSauGiam = tongtienmoi,
                thongBao = $"Áp dụng mã '{ma}' thành công!",
                ma = ma,
                pk_mkm = mkm.PK_MaKhuyenMai
            });
        }

        [HttpPost]
        public async Task<IActionResult> GuiDon(
        string ghiChu,
        int tongGiaTri,
        string hoTen,
        string soDienThoai,
        string diaChiChiTiet,
        int maKhuyenMai,
        string phuongThucThanhToan)
        {   

            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var tk = _dataContext.TaiKhoans.FirstOrDefault(x => x.Email == email);

            DateTime dateTime = DateTime.Now;
            DateTime dateTime2 = DateTime.Now.AddDays(3);

            DonHangModel donhang = new DonHangModel
            {
                ThoiGianDatHang = dateTime,
                TrangThaiDonHang = "Chờ Xác Nhận",
                GhiChuDonHang = ghiChu,
                DonViVanChuyen = "Shipper Công Ty",
                ThoiGianGiaoHangDuKien = dateTime2,
                TongGiaTri = tongGiaTri,
                LoiNhuan = 0,
                PhuongThucThanhToan = phuongThucThanhToan,
                TrangThaiThanhToan = false,
                NguoiNhan = hoTen,
                SDT = soDienThoai,
                DiaChi = diaChiChiTiet,
                FK_MaKhuyenMai = maKhuyenMai,
                FK_TaiKhoan = tk.PK_TaiKhoan
            };

            _dataContext.DonHangs.Add(donhang);
            _dataContext.SaveChanges();

            HttpContext.Session.Remove("DaApDungMa");

            int donHangId = donhang.PK_DonHang; // Giờ có giá trị rồi

            var ghang = _dataContext.GioHangs.FirstOrDefault(x => x.FK_TaiKhoan == tk.PK_TaiKhoan);
            var ctgh = _dataContext.ChiTietGioHangs.Where(x => x.FK_GioHang == ghang.PK_GioHang).ToList();

            // 3. Tạo danh sách chi tiết đơn hàng tương ứng
            var chiTietDonHang = ctgh.Select(g => new ChiTietDonHangModel
            {
                SoLuongSP = g.SoLuongSP,
                FK_DoChoi = g.FK_DoChoi,
                FK_DonHang = donhang.PK_DonHang
            }).ToList();

            _dataContext.ChiTietDonHangs.AddRange(chiTietDonHang);

            // Cập nhật số lượng bán và còn lại
            foreach (var item in chiTietDonHang)
            {
                var doChoi = await _dataContext.DoChois.FindAsync(item.FK_DoChoi);
                if (doChoi != null)
                {
                    doChoi.SoLuongDaBan += item.SoLuongSP;
                    doChoi.SoLuongConLai -= item.SoLuongSP;
                }
            }

            _dataContext.SaveChanges();

            // 6. Xóa chi tiết giỏ hàng sau khi đặt hàng
            _dataContext.ChiTietGioHangs.RemoveRange(ctgh);
            _dataContext.SaveChanges();

            return Ok(new { status = "ok" , iddonhang = donHangId});
        }

    }
}
