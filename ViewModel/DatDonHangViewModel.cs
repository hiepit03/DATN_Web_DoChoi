using Web_DoChoi.Models;

namespace Web_DoChoi.ViewModel
{
    public class DatDonHangViewModel
    {
        public string HoTen { get; set; }
        public string email { get; set; }
        public List<DiaChiModel> DiaChis { get; set; }
        public List<MaKhuyenMaiModel> MaKhuyenMais { get; set; }
    }
}
