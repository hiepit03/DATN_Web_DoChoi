using System.ComponentModel.DataAnnotations;

namespace Web_DoChoi.Models
{
    public class OrderInfoModel
    {
        [Key]
        public int PK_Order {  get; set; }
        public string FullName { get; set; }
        public string OrderId { get; set; }
        public string OrderInfor { get; set; }
        public string LoaiThanhToan { get; set; }
        public double Amount { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public int FK_DonHang {  get; set; }
        public bool TrangThai { get; set; } = false;
    }
}
