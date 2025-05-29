using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_DoChoi.Repository;

namespace Web_DoChoi.ViewComponents
{
    public class SoSanPhamGioHangViewComponent : ViewComponent
    {

        private readonly DataContext _dataContext;

        public SoSanPhamGioHangViewComponent(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var email = HttpContext.User.FindFirstValue(ClaimTypes.Email);

            var tk = await _dataContext.TaiKhoans.FirstOrDefaultAsync(p => p.Email == email);

            var gioHang = await _dataContext.GioHangs.FirstOrDefaultAsync(g => g.FK_TaiKhoan == tk.PK_TaiKhoan);

            var ctgh = await _dataContext.ChiTietGioHangs.Where(x=>x.FK_GioHang == gioHang.PK_GioHang).ToListAsync();

            int tongSoLuong = ctgh.Sum(x => x.SoLuongSP);

            return View(tongSoLuong);

        }

    }
}
