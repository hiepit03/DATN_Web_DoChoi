using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web_DoChoi.Validation;

namespace Web_DoChoi.Models
{
    public class SlideModel
    {
        [Key]
        public int PK_Slide { get; set; }
        public string HinhAnh { get; set; } = "noname.png";

        [NotMapped]
        [FileExtension]
        public IFormFile AnhUpLoad { get; set; }
    }
}
