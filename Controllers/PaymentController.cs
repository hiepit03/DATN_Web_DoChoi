using Microsoft.AspNetCore.Mvc;
using Web_DoChoi.Models;
using Web_DoChoi.Models.Vnpay;
using Web_DoChoi.Repository;
using Web_DoChoi.Services.Momo;
using Web_DoChoi.Services.Vnpay;

namespace Web_DoChoi.Controllers
{
    public class PaymentController : Controller
    {
        private IMomoService _momoService;
        private readonly IVnPayService _vnPayService;
        private readonly DataContext _dataContext;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger, DataContext dataContext, IMomoService momoService, IVnPayService vnPayService)
        {
            _logger = logger;
            _dataContext = dataContext;
            _momoService = momoService;
            _vnPayService = vnPayService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentMoMo(OrderInfoModel model)
        {   
             
            if (string.IsNullOrEmpty(model.OrderInfor) || string.IsNullOrEmpty(model.OrderId))
            {
                return BadRequest("Thiếu thông tin đơn hàng.");
            }

            model.ThoiGianTao = DateTime.Now;
            model.LoaiThanhToan = "MoMo";
            _dataContext.OrderInfos.Add(model);
            _dataContext.SaveChanges();

            var response = await _momoService.CreatePaymentMomo(model);
            return Redirect(response.PayUrl);
        }

        [HttpGet]
        public IActionResult PaymentCallBack()
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            return View(response);
        }

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {

            var ck = new OrderInfoModel
            {
                FullName = model.Name,
                OrderId = model.OrderId,
                OrderInfor = model.OrderDescription,
                Amount = model.Amount,
                LoaiThanhToan = "VNPay",
                ThoiGianTao = DateTime.Now,
                FK_DonHang = model.FK_DonHang
            };
            _dataContext.OrderInfos.Add(ck);
            _dataContext.SaveChanges();

            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }

        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            return Json(response);
        }


    }
}
