using System.ComponentModel.DataAnnotations;

namespace Web_DoChoi.Models
{
    public class LienHeModel
    {
        [Key]
        public int PK_LienHe { get; set; }
        public string HoTen {  get; set; }
        public string Email { get; set; }
        public string SDT { get; set; }
        public string NoiDung { get; set; }

    }
}
