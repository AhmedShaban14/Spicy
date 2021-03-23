using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spicy.Models.ViewModels
{
    public class CategoryAndSubGategoryViewModel
    {
        public IEnumerable<Category> Categories { get; set; }

        public SubCategory SubCategory { get; set; }
    }
}
