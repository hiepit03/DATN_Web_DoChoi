using Microsoft.AspNetCore.Mvc;
using Web_DoChoi.Repository;
using Web_DoChoi.Services.Vnpay;

namespace Web_DoChoi.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<CheckoutController> _logger;
        private readonly IVnPayService _vnPayService;

        public CheckoutController(ILogger<CheckoutController> logger, DataContext dataContext, IVnPayService vnPayService)
        {
            _logger = logger;
            _dataContext = dataContext;
            _vnPayService = vnPayService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult PaymentCallback(
             string partnerCode,
             string accessKey,
             string requestId,
             string amount,
             string orderId,
             string orderInfo,
             string orderType,
             string transId,
             string message,
             string localMessage,
             string responseTime,
             int errorCode,
             string payType,
             string extraData,
             string signature)
            {
                // Chỉ lấy thông tin cần thiết
                var paymentInfo = new Dictionary<string, string>
                    {
                        { "Order ID", orderId },
                        { "Amount", amount },
                        { "Order Info", orderInfo },
                        { "Message", message },
                        { "Transaction ID", transId },
                        { "Response Time", responseTime },
                        { "Pay Type", payType },
                        { "Error Code", errorCode.ToString() }
                    };

            var cck = _dataContext.OrderInfos.FirstOrDefault(x => x.OrderId == orderId);
            var dh = _dataContext.DonHangs.FirstOrDefault(x => x.PK_DonHang == cck.FK_DonHang);

            if (errorCode == 0)
            {
                dh.TrangThaiThanhToan = true;
                cck.TrangThai = true;
            }
            _dataContext.SaveChanges();

            // Đặt thông tin vào ViewBag
            ViewBag.PaymentInfo = paymentInfo;
            ViewBag.IsSuccess = errorCode == 0; 
                return View();
        }


        [HttpGet]
        public IActionResult PaymentCallbackVnpay(
             string vnp_TxnRef,
             string vnp_OrderInfo,
             string vnp_Amount,
             string vnp_ResponseCode,
             string vnp_TransactionNo,
             string vnp_PayDate,
             string vnp_SecureHash)
        {
            // Chỉ lấy thông tin cần thiết
            var paymentInfo = new Dictionary<string, string>
                {
                    { "Order ID", vnp_TxnRef },
                    { "Amount", vnp_Amount },
                    { "Order Info", vnp_OrderInfo },
                    { "Response Code", vnp_ResponseCode },
                    { "Transaction No", vnp_TransactionNo },
                    { "Payment Date", vnp_PayDate }
                };

            // Tìm đơn hàng tương ứng từ cơ sở dữ liệu
            var cck = _dataContext.OrderInfos.FirstOrDefault(x => x.OrderId == vnp_TxnRef);
            if (cck != null)
            {   
                var dh = _dataContext.DonHangs.FirstOrDefault(x => x.PK_DonHang == cck.FK_DonHang);
                if (dh != null)
                {
                    // Cập nhật trạng thái thanh toán nếu thanh toán thành công (vnp_ResponseCode = "00")
                    if (vnp_ResponseCode == "00")
                    {
                        dh.TrangThaiThanhToan = true;
                        cck.TrangThai = true;
                    }
                    else
                    {
                        dh.TrangThaiThanhToan = false;
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _dataContext.SaveChanges();
                }
            }

            // Đặt thông tin vào ViewBag để hiển thị trong View
            ViewBag.PaymentInfo = paymentInfo;
            ViewBag.IsSuccess = vnp_ResponseCode == "00"; // Kiểm tra nếu Response Code là "00" thì thanh toán thành công

            // Trả về View
            return View("PaymentResult");
        }


    }
}
