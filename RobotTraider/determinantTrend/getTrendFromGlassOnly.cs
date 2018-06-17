using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TradeObjects.Enums;
using static TradeObjects.QuikDataObj;

namespace determinantTrend.GetTrend
{
    public static class getTrendFromGlassOnly
    {
        public static Trend GetTrend(Glass glass, float K = 2)
        {
            var maxBuyQuantity = glass.BuyCells.Max(x => x.Qantity);
            var maxSaleQuantity = glass.SaleCells.Max(x => x.Qantity);

            if (maxBuyQuantity >= maxSaleQuantity * K)
            {
                var price = glass.BuyCells.First(x => x.Qantity == maxBuyQuantity).Price;
                return Trend.Bull;
            }
            else if (maxSaleQuantity >= maxBuyQuantity * K)
            {
                var price = glass.SaleCells.First(x => x.Qantity == maxSaleQuantity).Price;
                return Trend.Bear;
            }
            else
            {
                return Trend.Unstable;
            }
        }
    }
}
