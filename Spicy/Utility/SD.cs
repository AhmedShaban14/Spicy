using Spicy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spicy.Utility
{
    public static class SD
    {
        public const string ManagerUser = "Manager";
        public const string KitchenUser = "Kitchen";
        public const string FrontDiskUser = "FrontDiskr";
        public const string CustomerEndUser = "Customer";

        public const string ShoppingCartCount = "ShoppingCartCount";
        public const string ssCouponCode = "CouponCode";

        public const string StatusSubmitted = "Submitted";
        public const string StatusInProcess = "Being Prepared";
        public const string StatusReady = "Ready for Pickup";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusRejected = "Rejected";



        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static double DiscountPrice(Coupon coupon, double originalOrder)
        {
            if (coupon == null)
            {
                return originalOrder;
            }
            else
            {
                if (coupon.MinimalAmount > originalOrder)
                {
                    return originalOrder;

                }
                else
                {
                    if (int.Parse(coupon.CouponType) == (int)Coupon.Couponss.Doller)
                    {
                        return originalOrder - coupon.Discount;
                    }
                    else
                    {
                        return originalOrder - (originalOrder*(coupon.Discount/100));
                    }
                }
            }

        }
    }
}
