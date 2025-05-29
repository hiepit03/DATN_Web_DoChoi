using Web_DoChoi.Models;

namespace Web_DoChoi.ViewModel
{
    public class TrangChuViewModel
    {
        public List<SlideModel> Slides { get; set; }
        public List<DoChoiModel> DoChoi { get; set; }
        public List<DoChoiModel> DoChoiMoi { get; set; }
        public List<DoChoiModel> DoChoiBanChay { get; set; }
    }
}
