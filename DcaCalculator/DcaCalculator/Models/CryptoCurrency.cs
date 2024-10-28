using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Server.Models
{
    public class CryptoCurrency
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } 

        [Required] 
        [StringLength(10)]
        public string Symbol { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
