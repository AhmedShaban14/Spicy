using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spicy.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string NameOfUser { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string StreetAddress { get; set; }
    }
}
