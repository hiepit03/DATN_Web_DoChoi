using Web_DoChoi.Models;

namespace Web_DoChoi.ViewModel
{
    public class DonHangViewModel
    {
        public DonHangModel DonHang { get; set; }

        // Danh sách sản phẩm trong đơn hàng
        public List<ChiTietDonHangViewModel> ChiTietDonHangs { get; set; }
    }
}
