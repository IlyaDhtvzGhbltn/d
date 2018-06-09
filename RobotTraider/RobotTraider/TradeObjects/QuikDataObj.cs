using RobotTraider.TradeObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotTraider.QuikDataObj
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
    }

    public class ToQuikCommand
    {
        public C_Code? C_Code { get; set; }
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
        public Flag_Status flag_status { get; set; }
    }
}
