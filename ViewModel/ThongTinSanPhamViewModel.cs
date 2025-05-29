using Web_DoChoi.Models;

namespace Web_DoChoi.ViewModel
{
    public class ThongTinSanPhamViewModel
    {   
        public DoChoiModel dochoi {  get; set; }
        public List<DanhGiaModel> DanhGias { get; set; }
        public List<DoChoiModel> Dochoilienquan { get; set; }
    }
}
