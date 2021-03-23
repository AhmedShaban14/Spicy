using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Spicy.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public string ImageUrl { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }

        public int SpicyTypes { get; set; }

        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        [ForeignKey("SubCategoryId")]

        public SubCategory SubCategory { get; set; }
        public enum Spices
        {
            NotSPicy = 1,
            Spicy = 2,
            VerySpicy = 3
        }
    }

    
}
