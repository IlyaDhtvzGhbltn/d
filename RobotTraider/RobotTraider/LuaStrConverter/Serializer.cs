using RobotTraider.QuikDataObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotTraider.LuaStrConverter
{
    class Serializer
    {
        public static string QuikCommandToTransaction(ToQuikCommand QComm)
        {
            string transStr;
            if (QComm != null)
            {
                transStr = string.Empty;
                switch(QComm.C_Code)
                {
                    case TradeObjects.C_Code.Buy :
                        transStr = BuySale_Serialize(QComm);
                        break;
                    case TradeObjects.C_Code.Sale :
                        transStr = BuySale_Serialize(QComm);
                        break;
                    case TradeObjects.C_Code.Kill :
                        transStr = K_Serialize(QComm);
                        break;
                    case TradeObjects.C_Code.Wait :
                        transStr = "w";
                        break;
                    case TradeObjects.C_Code.MarkefSale :
                        transStr = MarketBuySale_Serialize(QComm, 'S');
                        break;
                    case TradeObjects.C_Code.MarketBuy:
                        transStr = MarketBuySale_Serialize(QComm, 'B');
                        break;
                }
                return transStr;
            }
            return null;
        }

        private static string BuySale_Serialize(ToQuikCommand QComm)
        {
            string B_transStr = string.Empty;
            B_transStr = string.Format
                (
                "CLASSCODE = {0}; " +
                "SECCODE = {1}; " +
                "ACTION = {2}; " +
                "ACCOUNT = {3}; " +
                "OPERATION = {4}; " +
                "QUANTITY = {5}; " +
                "TRANS_ID = {6}; " +
                "PRICE = {7};",
                QComm.ClassCode,
                QComm.SecCode,
                QComm.Action,
                QComm.Account,
                QComm.Operation,
                QComm.Qantity.ToString(),
                QComm.Trans_Id,
                QComm.Price.ToString()
                );
            return B_transStr;
        }

        private static string MarketBuySale_Serialize(ToQuikCommand QComm, char reverseTO)
        {
            string Market_transStr = string.Empty;
            Market_transStr = string.Format(
                "CLASSCODE = {0};"+
                "SECCODE = {1}; " +
                "ACTION = {2}; " +
                "ACCOUNT = {3}; " +
                "OPERATION = {4}; " +
                "QUANTITY = {5}; " +
                "TRANS_ID = {6}; " +
                "PRICE = {7};" + 
                "TYPE = M",
                QComm.ClassCode,
                QComm.SecCode,
                QComm.Action,
                QComm.Account,
                reverseTO,
                QComm.ThrowQantity,
                "1",
                0
                );
            return Market_transStr;
        }

        private static string K_Serialize(ToQuikCommand QComm)
        {
            string K_transStr = string.Empty;
            K_transStr = "k-" + QComm.Trans_Id;
            return K_transStr;
        }
    }
}
