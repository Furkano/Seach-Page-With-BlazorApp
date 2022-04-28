using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data
{
    public static class SelectableOrderBookTypes
    {
        public const string PriceLowToHigh = "priceLowToHigh";
        public const string PriceHighToLow = "priceHighToLow";
        public const string LikeCountHighToLow = "likeCountHighToLow";
        public const string LikeCountLowToHigh = "likeCountLowToHigh";
    }
}
