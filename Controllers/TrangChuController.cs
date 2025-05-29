using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using Web_DoChoi.Repository;
using Web_DoChoi.ViewModel;

namespace Web_DoChoi.Controllers
{
    public class TrangChuController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<TrangChuController> _logger;

        public TrangChuController(ILogger<TrangChuController> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {
            var dochoi = _dataContext.DoChois.Take(12).ToList();
            var dochoimoi = _dataContext.DoChois
                    .OrderByDescending(x => x.NgayNhapHang)
                    .Take(12)
                    .ToList();
            var dochoibanchay = _dataContext.DoChois
                   .OrderByDescending(x => x.SoLuongDaBan)
                   .Take(6)
                   .ToList();
            var slide = _dataContext.Slides.ToList();
            var tc = new TrangChuViewModel
            {
                Slides = slide,
                DoChoi = dochoi,
                DoChoiMoi = dochoimoi,
                DoChoiBanChay = dochoibanchay
            };
            return View(tc);
        }

        public IActionResult Details(int IdDoChoi)
        {
            var dochoi = _dataContext.DoChois.FirstOrDefault(x => x.PK_DoChoi == IdDoChoi);
            var danhgia = _dataContext.DanhGias
                            .Include(x => x.TaiKhoan)
                            .Where(x => x.FK_DoChoi == dochoi.PK_DoChoi)
                            .OrderByDescending(x => x.ThoiGianThem).ToList();
            var dochoilq = _dataContext.DoChois.Where(x => x.FK_DanhMuc == dochoi.FK_DanhMuc).ToList();
            var viewmodel = new ThongTinSanPhamViewModel
            {   
                dochoi = dochoi,
                DanhGias = danhgia,
                Dochoilienquan = dochoilq

            };
            return View(viewmodel);
        }

    }
}
