using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_DoChoi.Models;
using Web_DoChoi.Repository;
using Web_DoChoi.ViewModel;

namespace Web_DoChoi.ViewComponents
{
    public class DanhMucTimKiemViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;

        public DanhMucTimKiemViewComponent(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var danhMucs = await _dataContext.DanhMucs.ToListAsync();

            var viewModels = danhMucs.Select(d => new DanhMucTimKiemViewModel
            {
                DanhMuc = d,
                SoLuongSP = _dataContext.DoChois.Count(sp => sp.FK_DanhMuc == d.PK_DanhMuc)
            }).ToList();

            return View(viewModels);
        }
    }
}
