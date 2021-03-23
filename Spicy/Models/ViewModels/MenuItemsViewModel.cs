using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spicy.Models.ViewModels
{
    public class MenuItemsViewModel
    {
        public MenuItem MenuItem { get; set; }

        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<SubCategory> SubCategories { get; set; }

    }
}
