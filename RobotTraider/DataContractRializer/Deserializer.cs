using System;
using System.Collections.Generic;
using System.Linq;
using static TradeObjects.Enums;
using static TradeObjects.QuikDataObj;
using OrderRow = TradeObjects.QuikDataObj.BaseOrderRow;

namespace DataContractRializer
{
    public class Deserializer
    {
        public static Glass Glass(string StrGlass)
        {
            string[] StrDataGlass;
            Glass GlSs;

            if (StrGlass != null && StrGlass != string.Empty && StrGlass != "")
            {
                GlSs = new Glass();
                GlSs.SaleCells = new List<GlassCell>();
                GlSs.BuyCells = new List<GlassCell>();

                StrDataGlass = StrGlass.Split(';');
                int len = StrDataGlass.Length;

                try
                {
                    // Заявки на продажу
                    for (int j = len - 1; j > len / 2; j = j - 2)
                    {
                        GlSs.SaleCells.Add(new GlassCell
                        {
                            Price = Convert.ToSingle(StrDataGlass[j].Replace('.', ',')),
                            Qantity = Convert.ToInt32(StrDataGlass[j - 1])
                        });
                    }
                    // Заявки на покупку
                    for (int i = 0; i < (len / 2) - 1; i = i + 2)
                    {
                        GlSs.BuyCells.Add(new GlassCell
                        {
                            Price = Convert.ToSingle(StrDataGlass[i + 1].Replace('.', ',')),
                            Qantity = Convert.ToInt32(StrDataGlass[i])
                        });
                    }
                    // Спред
                    GlSs.Spread = new Spread()
                    {
                        SaleCell = GlSs.SaleCells.Last(),
                        BuyCell = GlSs.BuyCells[0]
                    };
                    return GlSs;
                }
                catch (FormatException)
                {
                    return null;
                }
            }
            return null;
        }
        public static List<OrderRow> Orders(string StrOrders)
        {
            string[] StrDataRows;
            List<OrderRow> OrdersRows;

            if (StrOrders != null && StrOrders != string.Empty && StrOrders != "")
            {
                StrDataRows = StrOrders.Split(';');
                OrdersRows = new List<OrderRow>();
                for (int i = 0; i < StrDataRows.Length; i += 5)
                {
                    if (StrDataRows[i] == "-" && i != StrDataRows.Length - 1)
                    {
                        try
                        {
                            OrdersRows.Add(new OrderRow
                            {
                                trans_id = StrDataRows[i + 1],
                                class_code = StrDataRows[i + 2],
                                sec_code = StrDataRows[i + 3],
                                flag_status = (Flag_Status)Convert.ToInt32(StrDataRows[i + 4])
                            });
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Deserialiser Exeption");
                        }
                    }
                }
                return OrdersRows;
            }
            return null;
        }
    }
}
