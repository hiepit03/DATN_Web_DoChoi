using Web_DoChoi.Models;

namespace Web_DoChoi.ViewModel
{
    public class ThongKeTongQuanViewModel
    {
        public int doanhthu {get; set;}
        public int sldonhang { get; set;}
        public int soluongspdaban { get; set;}
        public int soluongkhachhanh { get; set;}
        public List<ChartDataPointViewModel> ChartDataPoints { get; set; }
        public List<DoChoiModel> Topdochoibanchay {  get; set; }
    }
}
