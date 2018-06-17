using System.Collections.Generic;
using System.Linq;
using static TradeObjects.Enums;
using static TradeObjects.QuikDataObj;

namespace determinantTrend.BestPrice
{
    public static class getBestPricefromHighQuantity
    {
        public static Dictionary<Trend, float> getPrices(Glass glass)
        {
            var trendPrices = new Dictionary<Trend, float>();

            var maxBuyQuantity = glass.BuyCells.Max(x => x.Qantity);
            var maxSaleQuantity = glass.SaleCells.Max(x => x.Qantity);

            var priceBuy = glass.BuyCells.First(x => x.Qantity == maxBuyQuantity).Price;
            var priceSale = glass.SaleCells.First(x => x.Qantity == maxSaleQuantity).Price;

            trendPrices[Trend.Bull] = priceBuy;
            trendPrices[Trend.Bear] = priceSale;
            return trendPrices;
        }

    }
}
