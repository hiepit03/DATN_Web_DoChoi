using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_DoChoi.Controllers;
using Web_DoChoi.Repository;

namespace Web_DoChoi.ViewComponents
{
    public class DanhMucMenuViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;

        public DanhMucMenuViewComponent(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var danhMucs = await _dataContext.DanhMucs.ToListAsync();

            return View(danhMucs);
        }
    }
}
