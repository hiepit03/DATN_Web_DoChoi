using Microsoft.AspNetCore.Mvc;
using Web_DoChoi.Repository;

namespace Web_DoChoi.Controllers
{
    public class ChiNhanhController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<ChiNhanhController> _logger;

        public ChiNhanhController(ILogger<ChiNhanhController> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {   
            var chinhanh = _dataContext.ChiNhanhs.ToList();
            return View(chinhanh);
        }
    }
}
