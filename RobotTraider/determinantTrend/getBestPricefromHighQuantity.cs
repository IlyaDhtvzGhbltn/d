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
            Spread spred = new Spread { BuyCell = glass.BuyCells.First(), SaleCell = glass.SaleCells.Last() };
            float spredSalePrice = spred.SaleCell.Price;
            float spredBuyPrice = spred.BuyCell.Price;

            trendPrices[Trend.Bull] = spredBuyPrice;
            trendPrices[Trend.Bear] = spredSalePrice;
            return trendPrices;
        }

    }
}
