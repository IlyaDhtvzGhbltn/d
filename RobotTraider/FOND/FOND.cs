using TradeObjects.Strategies;
using System;
using System.Linq;
using TermException = QUIKException.TermException;
using ListOrderExceptopn = QUIKException.ListOrderException;
using static TradeObjects.QuikDataObj;
using static TradeObjects.Enums;
using DataContractRializer;
using Connector = TradeObjects.LuaConnection.LuaMMFConnector;


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
                throw new TermException();
            }

            private ToQuikCommand GetReturnedCommand(Glass glass)
            {
                switch (Position)
                {
                    case PositionStatus.notPosition:
                        return NewTransaction(glass);
                    
                }

                throw new TermException();
            }


            private ToQuikCommand NewTransaction(Glass glass)
            {
                var Bid = GetTrend(glass);
                if (Bid.Trend == Trend.Bull)
                {
                    SetBid(Bid);
                    return new ToQuikCommand(this.Account, this.ClassCode, this.SecCode)
                    {
                        Command_Code = Operation.Buy,
                        Price = Bid.Price,
                        Qantity = Bid.Quantity,
                        Operation = "B",
                        Trans_Id = LastTransId
                    };

                }
                else if (Bid.Trend == Trend.Bear)
                {
                    SetBid(Bid);
                    return new ToQuikCommand(this.Account, this.ClassCode, this.SecCode)
                    {
                        Command_Code = Operation.Sale,
                        Price = Bid.Price,
                        Qantity = Bid.Quantity,
                        Operation = "S",
                        Trans_Id = LastTransId,
                        ThrowQantity = this.CurrTransPrceQuant.Qantity

                    };

                }
                else if (Bid.Trend == Trend.Unstable)
                {
                    return new ToQuikCommand { Command_Code = Operation.Wait };
                }

                throw new TermException();
            }

            private ToQuikCommand WatchSendingTransaction(Glass glass)
            {
                var ListOrders = Deserializer.Orders(Orders.GetData());
                if (ListOrders.Count > 0)
                {
                    var TransactionStat = ListOrders.LastOrDefault().flag_status;
                    if (TransactionStat == Flag_Status.buy_confirm || TransactionStat == Flag_Status.sale_confirm || TransactionStat == Flag_Status.confirm)
                    {
                        
                    }
                    else
                    {
                        return ClosePosition_CancelBid_Wait(glass);
                    }
                }
                throw new ListOrderExceptopn();
            }


            private void ClearBid()
            {
                this.LastTransId = string.Empty;
                this.CurrTransPrceQuant = null;
            }

            private void SetBid(TrVal Bid)
            {
                this.CurrTransPrceQuant = new GlassCell
                {
                    Price = Bid.Price,
                    Qantity = Bid.Quantity
                };

                var rnd = new Random((int)DateTime.Now.Ticks);
                string TrId = rnd.Next(1, 2000000).ToString();
                this.LastTransId = TrId;
                LastOpertionTrend = Bid.Trend;
            }

            private TrVal GetTrend(Glass glass)
            {
                var maxBuyQuantity = glass.BuyCells.Max(x => x.Qantity);
                var maxSaleQuantity = glass.SaleCells.Max(x => x.Qantity);

                if (maxBuyQuantity >= maxSaleQuantity * K)
                {
                    var price = glass.BuyCells.First(x => x.Qantity == maxBuyQuantity).Price;
                    return new TrVal {Trend = Trend.Bull, Price = price, Quantity = 1 };
                }
                else if (maxSaleQuantity >= maxBuyQuantity * K)
                {
                    var price = glass.SaleCells.First(x => x.Qantity == maxSaleQuantity).Price;
                    return new TrVal { Trend = Trend.Bear, Price = price, Quantity = 1};
                }
                else
                {
                    return new TrVal { Trend = Trend.Unstable };
                }
            }

            private ToQuikCommand ClosePosition_CancelBid_Wait(Glass glass)
            {
                var newTrend = GetTrend(glass);
                if (this.LastOpertionTrend != newTrend.Trend)
                {
                    if (this.LastOpertionTrend == Trend.Bear)
                    {
                        ClearBid();
                        return new ToQuikCommand { Command_Code = Operation.CloseLong, ThrowQantity = CurrTransPrceQuant.Qantity };
                    }
                    else if (this.LastOpertionTrend == Trend.Bull)
                    {
                        ClearBid();
                        return new ToQuikCommand { Command_Code = Operation.CloseShort, ThrowQantity = CurrTransPrceQuant.Qantity };
                    }
                }
                else
                {

                    var minPriceGlas = glass.BuyCells.Min().Price;
                    var maxPriceGlas = glass.SaleCells.Max().Price;

                    if (this.CurrTransPrceQuant.Price < minPriceGlas || this.CurrTransPrceQuant.Price > maxPriceGlas)
                    {
                        string tmpTransId = LastTransId;
                        ClearBid();
                        return new ToQuikCommand()
                        {
                            Command_Code = Operation.Cancel,
                            Trans_Id = tmpTransId
                        };
                    }
                    else
                    {
                        return new ToQuikCommand
                        {
                            Command_Code = Operation.Wait
                        };
                    }
                }
                throw new Exception("Unexpected conditions");
            }

            private class TrVal
            {
                public Trend Trend { get; set; }
                public float Price { get; set; }
                public int Quantity { get; set; }
            }
        }
    }
}
