using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spicy.Models.ViewModels
{
    public class OrderHeaderShoppingCartViewModel
    {
        public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }

        public OrderHeader OrderHeader { get; set; }

    }
}
