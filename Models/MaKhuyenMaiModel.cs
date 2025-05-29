using System.ComponentModel.DataAnnotations;

namespace Web_DoChoi.Models
{
    public class MaKhuyenMaiModel
    {
        [Key]
        public int PK_MaKhuyenMai { get; set; }
        public string TenMaGiamGia { get; set; }
        public string MoTa {  get; set; }
        public int GiaTienApDung {  get; set; }
        public int GiaTienGiam {  get; set; }
        public DateTime ThoiGianBatDau {  get; set; }
        public DateTime ThoiGianKetThuc {  get; set; }

    }
}
