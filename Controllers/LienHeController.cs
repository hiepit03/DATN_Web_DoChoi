using Microsoft.AspNetCore.Mvc;
using Web_DoChoi.Models;
using Web_DoChoi.Repository;

namespace Web_DoChoi.Controllers
{
    public class LienHeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<LienHeController> _logger;

        public LienHeController(ILogger<LienHeController> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public IActionResult ThongTin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GuiLienHe(string HoTen, string Email, string SDT, string NoiDung)
        {

            // Kiểm tra đơn giản
            if (string.IsNullOrWhiteSpace(HoTen) || string.IsNullOrWhiteSpace(Email))
            {
                return BadRequest("Thiếu thông tin");
            }

            var lienhe = new LienHeModel
            {
                HoTen = HoTen,
                Email = Email,
                SDT = SDT,
                NoiDung = NoiDung
            };
            _dataContext.LienHes.Add(lienhe);
            _dataContext.SaveChanges();

            // In log kiểm tra
            Console.WriteLine($"Họ tên: {HoTen}, Email: {Email}, SDT: {SDT}, ND: {NoiDung}");

            // TODO: Lưu vào DB nếu muốn

            return Ok(new { message = "Gửi thành công" });

        }

    }
}
