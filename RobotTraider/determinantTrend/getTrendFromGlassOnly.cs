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
        public static Trend GetTrend(Glass glass, float K = 5)
        {
            //var maxBuyQuantity = glass.BuyCells.Max(x => x.Qantity);
            //var maxSaleQuantity = glass.SaleCells.Max(x => x.Qantity);

            Spread spred = new Spread { BuyCell = glass.BuyCells.First(), SaleCell = glass.SaleCells.Last() };
            float spredSalePrice = spred.SaleCell.Price;
            float spredBuyPrice = spred.BuyCell.Price;
            int spredSaleQuantity = spred.SaleCell.Qantity;
            int spredBuyQuantity = spred.BuyCell.Qantity;
            

            if (spredBuyQuantity >= spredSaleQuantity * K)
                 return Trend.Bull;
            else if (spredSaleQuantity >= spredBuyQuantity * K)
                return Trend.Bear;
            else
                return Trend.Unstable;
        }
    }
}
