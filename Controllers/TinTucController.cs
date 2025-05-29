using Microsoft.AspNetCore.Mvc;
using Web_DoChoi.Repository;

namespace Web_DoChoi.Controllers
{
    public class TinTucController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<TinTucController> _logger;

        public TinTucController(ILogger<TinTucController> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            var tintuc = _dataContext.TinTucs
                .OrderByDescending(t => t.ThoiGian) // hoặc t.NgayTao
                .ToList();
            return View(tintuc);
        }

        public IActionResult ChiTiet(int Id)
        {
            var cttt = _dataContext.TinTucs.Find(Id);
            return View(cttt);
        }
    }
}
