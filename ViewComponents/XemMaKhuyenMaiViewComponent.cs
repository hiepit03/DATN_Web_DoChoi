using Microsoft.AspNetCore.Mvc;
using Web_DoChoi.Repository;

namespace Web_DoChoi.ViewComponents
{
    public class XemMaKhuyenMaiViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;

        public XemMaKhuyenMaiViewComponent(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var now = DateTime.Now;
            var mkm = _dataContext.MaKhuyenMais
                .Where(x => x.ThoiGianBatDau <= now && now <= x.ThoiGianKetThuc)
                .ToList();
            return View(mkm);
        }
        
    }
}
