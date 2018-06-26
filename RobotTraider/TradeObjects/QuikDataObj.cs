using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TradeObjects.Enums;
using enums = TradeObjects.Enums;

namespace TradeObjects
{
    public class QuikDataObj
    {
        public class GlassCell : IComparable
        {
            public float Price { get; set; }
            public int Qantity { get; set; }

            public int CompareTo(object obj)
            {
                GlassCell cell = (GlassCell)obj;
                if (this.Price > cell.Price) return 1;
                if (this.Price < cell.Price) return -1;
                else return 0;
            }
        }

        public class Spread
        {
            public GlassCell SaleCell { get; set; }
            public GlassCell BuyCell { get; set; }
        }

        public class Glass
        {
            public Spread Spread { get; set; }
            public List<GlassCell> BuyCells { get; set; }
            public List<GlassCell> SaleCells { get; set; }

            public float MiddleQuantity()
            {
                float bidCount = 0;
                float totalQuantity = 0;
                bidCount += this.BuyCells.Count;
                bidCount += this.SaleCells.Count;
                totalQuantity += BuyCells.Sum(x=>x.Qantity);
                totalQuantity += SaleCells.Sum(x=>x.Qantity);
                return totalQuantity / bidCount;
            }
            public Tuple<Trend, float, int> GetBidQuantMoreMidd(float midd, int K)
            {
                var SaleQuant = this.SaleCells.Max(x=>x.Qantity);
                var BuyQant = this.BuyCells.Max(x => x.Qantity);
                GlassCell TargetCell;
                Trend TargTrend;

                if (BuyQant > SaleQuant)
                {
                    TargetCell = this.BuyCells.FirstOrDefault(x => x.Qantity == BuyQant);
                    TargTrend = Trend.Bull;
                }
                else
                {
                    TargetCell = this.SaleCells.FirstOrDefault(x => x.Qantity == SaleQuant);
                    TargTrend = Trend.Bear;
                }

                if (TargetCell.Qantity >= midd * K)
                {
                    return new Tuple<Trend, float, int>(TargTrend, TargetCell.Price, TargetCell.Qantity);
                }
               

                return null;
            }

            public bool IsBidINSpreadByPrice(float price)
            {
                try
                {
                    int indexInSale = this.SaleCells.FindIndex(x => x.Price == price);
                    int indexInBuy = this.BuyCells.FindIndex(x => x.Price == price);
                    int lastSpredIndexInSale = BuyCells.Count - 1;
                    if (indexInBuy == 0 || indexInSale == lastSpredIndexInSale)
                    {
                        return true;
                    }
                    else
                        return false;
                }
                catch (Exception)
                {

                    return false;
                }
            }

            public float getActualPrice()
            {
                return SaleCells.Last().Price;
            }

        }

        public class ToQuikCommand
        {
            public enums.Operation? Command_Code { get; set; }
            public string ClassCode { get; set; }
            public string SecCode { get; set; }
            public string Action { get; set; }
            public string Account { get; set; }
            public string Operation { get; set; }
            public float Price { get; set; }
            public int Qantity { get; set; }
            public string Trans_Id { get; set; }
            public string Oreder_Key { get; set; }

            public int ThrowQantity { get; set; }

            public ToQuikCommand(string account, string classCode, string secCode)
            {
                Account = account;
                ClassCode = classCode;
                SecCode = secCode;
                Action = "NEW_ORDER";
            }
            public ToQuikCommand() { }
        }

        public class BaseOrderRow
        {
            public string trans_id { get; set; }
            public string class_code { get; set; }
            public string sec_code { get; set; }
            public enums.Flag_Status flag_status { get; set; }
        }
    }
}
