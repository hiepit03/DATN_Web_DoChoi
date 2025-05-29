using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_DoChoi.Repository;
using Web_DoChoi.ViewModel;

namespace Web_DoChoi.ViewComponents
{
    public class NhaSanXuatTimKiemViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;

        public NhaSanXuatTimKiemViewComponent(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var nsx = await _dataContext.NhaSanXuats.ToListAsync();

            var viewModels = nsx.Select(d => new NhaSanXuatTimKiemViewModel
            {
                nsx = d,
                SoLuongSP = _dataContext.DoChois.Count(sp => sp.FK_NhaSanXuat == d.PK_NSX)
            }).ToList();

            return View(viewModels);
        }

    }
}
