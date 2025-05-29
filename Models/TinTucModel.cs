using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web_DoChoi.Validation;

namespace Web_DoChoi.Models
{
    public class TinTucModel
    {
        [Key]
        public int PK_TinTuc { get; set; }
        public string TenTinTuc {  set; get; }
        public DateTime ThoiGian { set; get; }
        public string HinhAnh { set; get; } = "noname.png";
        [NotMapped]
        [FileExtension]
        public IFormFile AnhUpLoad { get; set; }
        public string MoTa {  set; get; }

    }
}
