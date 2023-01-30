using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PaypalLab.Models
{
    public class Product
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Price { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        public string Image { get; set; }
    }
}
