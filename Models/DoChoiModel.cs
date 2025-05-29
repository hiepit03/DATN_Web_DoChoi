using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web_DoChoi.Validation;

namespace Web_DoChoi.Models
{
    public class DoChoiModel
    {
        [Key]
        public int PK_DoChoi { get; set; }
        public string TenDoChoi { get; set; }
        public string AnhDoChoi { get; set; } = "noname.png";

        [NotMapped]
        [FileExtension]
        public IFormFile AnhUpLoad { get; set; }

        public int GiaNhapHang { get; set; }
        public int GiaBan {  get; set; }
        public int PhanTramKhuyenMai {  get; set; }
        public int SoLuongDaBan { get; set; }
        public int SoLuongConLai { get; set; }
        public string MoTa {  get; set; }
        public DateTime NgayNhapHang { get; set; }
        public int DoTuoiGioiHan {  get; set; }
        public float SoSao {  get; set; }
        public int SoLuotDanhGia {  get; set; }

        public int FK_DanhMuc { get; set; }

        [ForeignKey("FK_DanhMuc")]
        public DanhMucModel? DanhMuc { get; set; }

        public int FK_NhaSanXuat { get; set; }

        [ForeignKey("FK_NhaSanXuat")]
        public NhaSanXuatModel? NhaSanXuat { get; set; }
    }
}
