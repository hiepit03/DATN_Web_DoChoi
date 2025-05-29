using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web_DoChoi.Models;
using Web_DoChoi.Repository;

namespace Web_DoChoi.Controllers
{
    public class DiaChiController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<DiaChiController> _logger;

        public DiaChiController(ILogger<DiaChiController> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var tk = _dataContext.TaiKhoans.FirstOrDefault(x => x.Email == email);
            var diachi = _dataContext.DiaChis.Where(x => x.FK_TaiKhoan == tk.PK_TaiKhoan).ToList();
            ViewBag.idtaikhoan = tk.PK_TaiKhoan;
            return View(diachi);
        }

        //thêm
        [HttpGet]
        public IActionResult Create(int idtaikhoan)
        {
            ViewBag.idtaikhoan = idtaikhoan;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiaChiModel sl)
        {
            if (ModelState.IsValid)
            {   
                if(sl.MacDinh == true)
                {
                    // Lấy danh sách địa chỉ của tài khoản
                    var allDiaChi = _dataContext.DiaChis
                        .Where(x => x.FK_TaiKhoan == sl.FK_TaiKhoan)
                        .ToList();

                    // Set tất cả là false
                    foreach (var diachi in allDiaChi)
                    {
                        diachi.MacDinh = false;
                    }
                }

                //them du lieu
                _dataContext.DiaChis.Add(sl);
                await _dataContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Model có 1 số lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string err = string.Join("\n", errors);
                return BadRequest(err);

            }
        }

        [HttpPost]
        public IActionResult XoaDiaChi(int iddiachi)
        {
            var diachi = _dataContext.DiaChis.Find(iddiachi);
            _dataContext.DiaChis.Remove(diachi);
            _dataContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SetDiaChiMacDinh(int iddiachi, int idtaikhoan)
        {
            // Lấy danh sách địa chỉ của tài khoản
            var allDiaChi = _dataContext.DiaChis
                .Where(x => x.FK_TaiKhoan == idtaikhoan)
                .ToList();

            // Set tất cả là false
            foreach (var diachi in allDiaChi)
            {
                diachi.MacDinh = false;
            }

            var dcht = _dataContext.DiaChis.Find(iddiachi);
            dcht.MacDinh = true;

            _dataContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
