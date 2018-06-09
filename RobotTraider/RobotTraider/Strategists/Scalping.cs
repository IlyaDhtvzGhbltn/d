using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RobotTraider.Logs;
using RobotTraider.LuaStrConverter;
using RobotTraider.QuikDataObj;
using RobotTraider.TradeObjects;
using Connector = RobotTraider.LuaConnection.LuaMMFConnector;

namespace RobotTraider.Strategists.Scalping
{

    // All transaction start as { ..
    // if not, lua interpretired it like hold

    class GlassScalping : Istrategies
    {
        private AppLoger Log = new AppLoger("App_Log2.txt");
        private double Deviation { get; set; }
        private string ClassCode { get; set; }
        private string SecCode { get; set; }
        private string Account { get; set; }
        private Connector Orders { get; set; }
        private DateTime LastTrans { get; set; }

        private GlassCell CurrTransPrceQuant { get; set; }
        private List<BaseOrderRow> ListOrders { get; set; }
        private string LastTransId = string.Empty;
        Random rnd;

        private float K_ { get; }

        private Trend LastOpertionTrend { get; set; }


        public GlassScalping(string ClassCode, string SecCode, string Account, string ordConnMmfName, float K = 2)
        {
            this.ClassCode = ClassCode;
            this.SecCode = SecCode;
            this.Account = Account;
            Orders = new Connector(ordConnMmfName);
            K_ = K;
            LastOpertionTrend = Trend.Unstable;
        }
       

        public ToQuikCommand[] GetSolution(object info)
        {
            if (info != null)
            {
                var gls = ((Glass)info);
                if (gls != null)
                {

                    ToQuikCommand[] command = GetReturnedCommand(gls);
                    return command;
                }
                else return null;
            }
            else return null;
        }

        private ToQuikCommand[] GetReturnedCommand(Glass glass)
        {
            var maxBuyQuantity = glass.BuyCells.Max(x => x.Qantity);
            var maxSaleQuantity = glass.SaleCells.Max(x => x.Qantity);

            var TotalQuantityByu = glass.BuyCells.Sum(x => x.Qantity);
            var TotalQuantitySale = glass.SaleCells.Sum(x => x.Qantity);

                if (LastTransId == string.Empty)
                {
                    Log.WriteLogMessage($"No transaction");

                    if (maxBuyQuantity >= maxSaleQuantity * K_)
                    {
                    if (LastOpertionTrend == Trend.Unstable || LastOpertionTrend == Trend.Bear)
                    {
                        Log.WriteLogMessage("Time to Buy!");

                        var price = glass.BuyCells.First(x => x.Qantity == maxBuyQuantity).Price;
                        var quantity = glass.BuyCells.First(x => x.Qantity == maxBuyQuantity).Qantity;
                        var MyQuantity = 10;//Convert.ToInt32(Math.Round((double)(quantity / 100)));



                        this.CurrTransPrceQuant = new GlassCell
                        {
                            Price = price,
                            Qantity = MyQuantity
                        };

                        rnd = new Random((int)DateTime.Now.Ticks);
                        string TrId = rnd.Next(1, 2000000).ToString();
                        LastTransId = TrId;
                        Log.WriteLogMessage($"-Buy Command- was Reset! Last Id = {LastTransId}!");

                        LastOpertionTrend = Trend.Bull;
                        return new ToQuikCommand[]
                        {
                            new ToQuikCommand(this.Account, this.ClassCode, this.SecCode)
                            {
                                C_Code = C_Code.MarketBuy,
                                ThrowQantity = this.CurrTransPrceQuant.Qantity
                            },
                            new ToQuikCommand(this.Account, this.ClassCode, this.SecCode)
                            {
                                C_Code = C_Code.Buy,
                                Price = price,
                                Qantity = MyQuantity,
                                Operation = "B",
                                Trans_Id = LastTransId
                            }
                        };
                        }
                    }
                    else if (maxSaleQuantity >= maxBuyQuantity * K_)
                    {
                        if (LastOpertionTrend == Trend.Unstable || LastOpertionTrend == Trend.Bull)
                        {
                            Log.WriteLogMessage("Time to Sale!");

                            var price = glass.SaleCells.First(x => x.Qantity == maxSaleQuantity).Price;
                            var quantity = glass.SaleCells.First(x => x.Qantity == maxSaleQuantity).Qantity;
                        var MyQuantity = 10;// Convert.ToInt32(Math.Round((double)(quantity / 100)));


                            this.CurrTransPrceQuant = new GlassCell
                            {
                                Price = price,
                                Qantity = MyQuantity
                            };

                            rnd = new Random((int)DateTime.Now.Ticks);
                            string TrId = rnd.Next(1, 2000000).ToString();
                            LastTransId = TrId;
                            Log.WriteLogMessage($"-Sale Command- was Reset! Last Id = {LastTransId}!");


                            LastOpertionTrend = Trend.Bear;
                            return new ToQuikCommand[] {
                                new ToQuikCommand
                                {
                                    C_Code = C_Code.MarkefSale,
                                    ThrowQantity = this.CurrTransPrceQuant.Qantity
                                },
                                new ToQuikCommand (this.Account, this.ClassCode, this.SecCode)
                                {
                                    C_Code = C_Code.Sale,
                                    Price = price,
                                    Qantity = MyQuantity,
                                    Operation = "S",
                                    Trans_Id = LastTransId,
                                    ThrowQantity = this.CurrTransPrceQuant.Qantity
                                }
                        };
                        }
                    }
                    else
                    {
                    Log.WriteLogMessage("Transaction is Empty. Wait conditions. Set -Wait Command-");
                    return new ToQuikCommand[] { new ToQuikCommand { C_Code = C_Code.Wait } };
                }
            }
                else
                {
                    ListOrders = Deserializer.Orders(Orders.GetData());

                    if (ListOrders != null)
                    {
                        if (ListOrders.Count > 0)
                        {
                            var LastOrder = ListOrders.LastOrDefault();

                            if (LastOrder != null)
                            {
                                var orderStat = LastOrder.flag_status;

                                if (orderStat == Flag_Status.buy_confirm ||
                                    orderStat == Flag_Status.sale_confirm ||
                                    orderStat == Flag_Status.confirm)
                                {
                                    Log.WriteLogMessage($"Last Order Status of {LastTransId} trn is complete!!! --- Start again");
                                    this.LastTransId = string.Empty;
                                    this.CurrTransPrceQuant = null;
                                }
                                else
                                {
                                    Log.WriteLogMessage("Last Order Status is still Active");

                                    var minPriceGlas = glass.BuyCells.Min().Price;
                                    var maxPriceGlas = glass.SaleCells.Max().Price;

                                    if (this.CurrTransPrceQuant.Price < minPriceGlas
                                        ||
                                        this.CurrTransPrceQuant.Price > maxPriceGlas
                                        )
                                        {
                                            Log.WriteLogMessage($"Price of Transaction № :{LastTransId} is too big or to small now.--- Kill transn start again");
                                            string tmpTransId = LastTransId;
                                            this.LastTransId = string.Empty;
                                            this.CurrTransPrceQuant = null;

                                            LastOpertionTrend = Trend.Unstable;

                                            return new ToQuikCommand[]
                                            {
                                                new ToQuikCommand()
                                                {
                                                C_Code = C_Code.Kill,
                                                Trans_Id = tmpTransId
                                                }
                                            };
                                    }
                                    else
                                    {
                                        Log.WriteLogMessage("Price is normal, wait ...");
                                        return new ToQuikCommand[]
                                        {
                                            new ToQuikCommand{ C_Code = C_Code.Wait}
                                        };
                                    }
                                }
                            }
                            else
                            {
                                Log.WriteLogMessage(" Last Order = null");
                                return new ToQuikCommand[] 
                                {
                                    new ToQuikCommand{ C_Code = C_Code.Wait }
                                };
                            }
                        }
                        else
                        {
                            Log.WriteLogMessage("Orders Collection = 0 or <= 0");
                            return new ToQuikCommand[] { new ToQuikCommand { C_Code = C_Code.Wait } };
                        }
                    }
                    else
                    {
                        Log.WriteLogMessage("ListOrders Collection = null");
                    return new ToQuikCommand[] { new ToQuikCommand { C_Code = C_Code.Wait } };
                }
            }
            return null;
        }
    }
}
