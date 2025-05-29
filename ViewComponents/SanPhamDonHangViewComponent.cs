using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_DoChoi.Repository;
using Web_DoChoi.ViewModel;

namespace Web_DoChoi.ViewComponents
{
    public class SanPhamDonHangViewComponent : ViewComponent
    {

        private readonly DataContext _dataContext;

        public SanPhamDonHangViewComponent(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var email = HttpContext.User.FindFirstValue(ClaimTypes.Email);

            var tk = await _dataContext.TaiKhoans.FirstOrDefaultAsync(p => p.Email == email);

            var gioHang = await _dataContext.GioHangs.FirstOrDefaultAsync(g => g.FK_TaiKhoan == tk.PK_TaiKhoan);

            var spgh = await _dataContext.ChiTietGioHangs.Where(p => p.FK_GioHang == gioHang.PK_GioHang).Include(p => p.DoChoi).ToListAsync();

            var viewModelList = spgh.Select(p => new SanPhamGioHangViewModel
            {
                ID_GioHang = gioHang.PK_GioHang,
                ChiTietGioHangId = p.PK_ChiTietGioHang,
                HinhAnh = p.DoChoi.AnhDoChoi,
                TenDoChoi = p.DoChoi.TenDoChoi,
                GiaDoChoi = p.DoChoi.GiaBan,
                SoLuongSP = p.SoLuongSP
            }).ToList();


            return View(viewModelList);

        }

    }
}
