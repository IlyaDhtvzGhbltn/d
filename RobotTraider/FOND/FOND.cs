using TradeObjects.Strategies;
using System;
using TermException = QUIKException.TermException;
using static TradeObjects.QuikDataObj;
using static TradeObjects.Enums;
using Connector = TradeObjects.LuaConnection.LuaMMFConnector;
using determinantTrend.BestPrice;
using determinantTrend.GetTrend;
using determinantTrend.BestQuantity;

using QUIKException;
using DataContractRializer;
using System.Linq;

namespace TradingStrategistQUIK
{
    public class SBER_actio 
    {
        public class MyFirstGlassScalping : Istrategies
        {
            // private AppLoger Log = new AppLoger("App_Log2.txt");
            private double Deviation { get; set; }
            private string ClassCode { get; set; }
            private string SecCode { get; set; }
            private string Account { get; set; }
            private Connector Orders { get; set; }
            private GlassCell CurrTransPrceQuant { get; set; }
            private string LastTransId = string.Empty;
            private float K = 1.4f;

            private Trend LastOpertionTrend { get; set; }
            private PositionStatus Position { get; set; }

            public MyFirstGlassScalping(string ClassCode, string SecCode, string Account, string ordConnMmfName)
            {
                this.ClassCode = ClassCode;
                this.SecCode = SecCode;
                this.Account = Account;
                this.Orders = new Connector(ordConnMmfName);
                LastOpertionTrend = Trend.Unstable;
                Position = PositionStatus.notPosition;
            }


            public ToQuikCommand GetToQUIKCommand(object info)
            {

                if (info != null)
                {
                    var gls = ((Glass)info);
                    if (gls != null)
                    {
                        ToQuikCommand command = GetReturnedCommand(gls);
                        return command;
                    }
                    else return null;
                }
                else
                {
                    return null;
                }
                throw new TermException();
            }

            private ToQuikCommand GetReturnedCommand(Glass glass)
            {
                switch (Position)
                {
                    case PositionStatus.notPosition:
                        return NewTransaction(glass);

                    case PositionStatus.SendBidLong:
                        return SendTransLONG(glass);

                    case PositionStatus.SendBidShort:
                        return SendTransSHORT(glass);

                    case PositionStatus.OpenPositionLONG:
                        return EnterINLongPosition(glass);

                    case PositionStatus.OpenPositionSHORT:
                        return EnterINShortPosition(glass);
                }
                throw new TermException();
            }

            private ToQuikCommand NewTransaction(Glass glass)
            {
                BidObject Bid = getBidObj(glass);

                if (Bid != null)
                {
                    if (Bid.Trend == Trend.Bull)
                    {
                        SetBid(Bid);
                        Position = PositionStatus.SendBidLong;
                        return new ToQuikCommand(this.Account, this.ClassCode, this.SecCode)
                        {
                            Command_Code = Operation.Buy,
                            Price = Bid.Price,
                            Qantity = Bid.Quantity,
                            Operation = "B",
                            Trans_Id = this.LastTransId
                        };

                    }
                    else if (Bid.Trend == Trend.Bear)
                    {
                        SetBid(Bid);
                        Position = PositionStatus.SendBidShort;
                        return new ToQuikCommand(this.Account, this.ClassCode, this.SecCode)
                        {
                            Command_Code = Operation.Sale,
                            Price = Bid.Price,
                            Qantity = Bid.Quantity,
                            Operation = "S",
                            Trans_Id = LastTransId,
                        };
                    }
                    else if (Bid.Trend == Trend.Unstable)
                    {
                        Position = PositionStatus.notPosition;
                        return new ToQuikCommand { Command_Code = Operation.Wait };
                    }
                    else return null;
                }
                return new ToQuikCommand { Command_Code = Operation.Wait };
            }

            private ToQuikCommand SendTransSHORT(Glass glass)
            {
                bool IsInGlass = IsPriceBidInGlass(glass);
                bool IsPositOpen = IsPositionOpen();
                var trend = getTrendFromGlassOnly.GetTrend(glass);

                if (trend == Trend.Bull)
                {
                    var ThrQuant = this.CurrTransPrceQuant.Qantity;
                    ClearBid();
                    return new ToQuikCommand(account: this.Account, classCode: this.ClassCode, secCode: this.SecCode)
                    {
                        Command_Code = Operation.CloseShort,
                        ThrowQantity = ThrQuant
                    };
                }

                if (IsInGlass == false)
                {
                    string Id = this.LastTransId;
                    ClearBid();
                    return new ToQuikCommand { Command_Code = Operation.Cancel, Trans_Id = Id };
                }

                if (IsPositOpen)
                {
                    this.Position = PositionStatus.OpenPositionSHORT;
                }
                return new ToQuikCommand { Command_Code = Operation.Wait };
            }

            private ToQuikCommand SendTransLONG(Glass glass)
            {
                bool IsInGlass = IsPriceBidInGlass(glass);
                bool IsPositOpen = IsPositionOpen();
                var trend = getTrendFromGlassOnly.GetTrend(glass);

                if (trend == Trend.Bear)
                {
                    var ThrQuant = this.CurrTransPrceQuant.Qantity;
                    ClearBid();
                    return new ToQuikCommand(account: this.Account, classCode: this.ClassCode, secCode: this.SecCode)
                    {
                        Command_Code = Operation.CloseLong,
                        ThrowQantity = ThrQuant
                    };
                }
                if (IsInGlass == false)
                {
                    string Id = this.LastTransId;
                    ClearBid();
                    return new ToQuikCommand { Command_Code = Operation.Cancel, Trans_Id = Id };
                }
                if (IsPositOpen)
                {
                    this.Position = PositionStatus.OpenPositionLONG;
                }
                return new ToQuikCommand { Command_Code = Operation.Wait };
            }

            private ToQuikCommand EnterINLongPosition(Glass glass)
            {
                var trnd = getTrendFromGlassOnly.GetTrend(glass);
                if (trnd == Trend.Bull || trnd == Trend.Unstable)
                    return new ToQuikCommand { Command_Code = Operation.Wait };
                else
                {
                    var Quantity = CurrTransPrceQuant.Qantity;
                    ClearBid();
                    return new ToQuikCommand(account: this.Account, classCode: this.ClassCode, secCode: this.SecCode)
                    {
                        Command_Code = Operation.CloseLong,
                        ThrowQantity = Quantity
                    };
                }
            }

            private ToQuikCommand EnterINShortPosition(Glass glass)
            {
                var trnd = getTrendFromGlassOnly.GetTrend(glass);
                if (trnd == Trend.Bear)
                    return new ToQuikCommand { Command_Code = Operation.Wait };
                else
                {
                    var Quantity = CurrTransPrceQuant.Qantity;
                    ClearBid();
                    return new ToQuikCommand(account: this.Account, classCode: this.ClassCode, secCode: this.SecCode)
                    {
                        Command_Code = Operation.CloseShort,
                        ThrowQantity = Quantity
                    };
                }
            }



            private void ClearBid()
            {
                string tmpId = this.LastTransId;
                this.LastTransId = string.Empty;
                this.CurrTransPrceQuant = null;
                this.Position = PositionStatus.notPosition;
            }

            private void SetBid(BidObject Bid)
            {
                this.CurrTransPrceQuant = new GlassCell
                {
                    Price = Bid.Price,
                    Qantity = Bid.Quantity
                };

                var rnd = new Random((int)DateTime.Now.Ticks);
                string TrId = rnd.Next(1, 2000000).ToString();
                this.LastTransId = TrId;
                this.LastOpertionTrend = Bid.Trend;
            }

            private BidObject getBidObj(Glass glass)
            {
                var trend = getTrendFromGlassOnly.GetTrend(glass);
                if (trend != Trend.Unstable)
                {
                    var price = getBestPricefromHighQuantity.getPrices(glass)[trend];
                    var qantity = getBestQuantityFromGlassOnly.getQuantity(glass);
                    return new BidObject { Trend = trend, Price = price, Quantity = qantity };
                }
                return null;
            }
            


            private bool IsPriceBidInGlass(Glass glass)
            {
                var minPriceGlas = glass.BuyCells.Min().Price;
                var maxPriceGlas = glass.SaleCells.Max().Price;

                if (this.CurrTransPrceQuant.Price < minPriceGlas ||
                    this.CurrTransPrceQuant.Price > maxPriceGlas)
                {
                    return false;
                }
                else
                    return true;
            }

            private bool IsPositionOpen()
            {
                var ListOrders = Deserializer.Orders(Orders.GetData());
                if (ListOrders.Count > 0)
                {
                    var TransactionStat = ListOrders.LastOrDefault().flag_status;

                    if (TransactionStat == Flag_Status.buy_kill || TransactionStat == Flag_Status.sale_kill)
                        return false;
                    if (TransactionStat == Flag_Status.buy_confirm || TransactionStat == Flag_Status.sale_confirm || TransactionStat == Flag_Status.confirm)
                    {
                        return true;
                    }
                    else
                       return false;
                }
                throw new ListOrderException();
            }

            private class BidObject
            {
                public Trend Trend { get; set; }
                public float Price { get; set; }
                public int Quantity { get; set; }
            }
        }
    }
}
