using static TradeObjects.Enums;
using static TradeObjects.QuikDataObj;

namespace DataContractRializer
{
    public class Serializer
    {
        public static string QuikCommandToTransaction(ToQuikCommand QComm)
        {
            string transStr;
            if (QComm != null)
            {
                transStr = string.Empty;
                switch (QComm.Command_Code)
                {
                    case Operation.Buy:
                        transStr = BuySale_Serialize(QComm);
                        break;
                    case Operation.Sale:
                        transStr = BuySale_Serialize(QComm);
                        break;
                    case Operation.Cancel:
                        transStr = K_Serialize(QComm);
                        break;
                    case Operation.Wait:
                        transStr = "w";
                        break;
                    case Operation.BuyInMarket:
                        transStr = MarketBuySale_Serialize(QComm, 'B');
                        break;
                    case Operation.SaleInMarket:
                        transStr = MarketBuySale_Serialize(QComm, 'S');
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
                "CLASSCODE = {0};" +
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
                QComm.Qantity,
                QComm.Trans_Id,
                0
                );
            return Market_transStr;
        }

        private static string K_Serialize(ToQuikCommand QComm)
        {
            string K_transStr = "k-" + QComm.Trans_Id;
            return K_transStr;
        }
    }
}
