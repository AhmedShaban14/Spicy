using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spicy.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }

        [Required]
        public string CouponType { get; set; }

        public enum Couponss { Percent ,Doller  }
        
        [Required]
        public double Discount { get; set; }
        
        [Required]
        public double MinimalAmount { get; set; }

        public bool IsActice { get; set; }
    }
}
