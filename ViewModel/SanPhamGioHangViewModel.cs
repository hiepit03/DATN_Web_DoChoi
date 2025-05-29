namespace Web_DoChoi.ViewModel
{
    public class SanPhamGioHangViewModel
    {   
        public int ID_GioHang { get; set; }
        public int ChiTietGioHangId { get; set; }
        public string HinhAnh {get; set;}
        public int SoLuongSP { get; set; }
        public string TenDoChoi { get; set; }  // Ví dụ, chỉ lấy tên sản phẩm
        public decimal GiaDoChoi { get; set; } // Nếu bạn cần giá
    }
}
