using System;
using System.Threading;
using System.Windows.Forms;
using Istrategies = TradeObjects.Strategies.Istrategies;
using TradingStrategistQUIK;
using Connector = TradeObjects.LuaConnection.LuaMMFConnector;
using static TradeObjects.QuikDataObj;
using DataContractRializer;

namespace RobotTraider
{
    public partial class Form1 : Form
    {
        // Ключевое слово "volatile" гарантирует, что в переменной в любое время и при обращении из любого потока будет актуальное значение
        public volatile bool Run = true;
        Connector CommandCtor;
        Connector GlassCtor;
        delegate void Print(string glasscellprice);


        Istrategies Scalp;
        public Form1()
        {
            InitializeComponent();
            GlassCtor = new Connector("TerminalQuote");
            CommandCtor = new Connector("QUIKCommand");
            Scalp = new SBER_actio.MyFirstGlassScalping("QJSIM", "SBER", "NL0011100043", "Orders");
            BidSolution(GlassCtor, Scalp);
        }

        private void BidSolution(Connector ctror, Istrategies Strateg)
        {
            new Thread(() =>
            {
                string QuoteStr = string.Empty;
                while (Run)
                {
                    QuoteStr = ctror.GetData();
                    var glass = Deserializer.Glass(QuoteStr);
                    FillVisual(glass);
                    ToQuikCommand BidComms = Strateg.GetToQUIKCommand(glass);
                    var LuaCommand = Serializer.QuikCommandToTransaction(BidComms);
                    CommandCtor.SendData(LuaCommand);
                    Thread.Sleep(1);
                    
                }
            })
            .Start();
        }

        private void FillVisual(Glass glass)
        {
            if (glass != null)
            {
                Invoke(new Print((s) => Salestb.Text = string.Empty), "");
                foreach (var item in glass.SaleCells)
                {
                    Invoke(new Print((s) => Salestb.Text += s), item.Price.ToString() + Environment.NewLine);
                }
                Invoke(new Print((s) => Buystb.Text = string.Empty), "");

                foreach (var item in glass.BuyCells)
                {
                    Invoke(new Print((s) => Buystb.Text += s), item.Price.ToString() + Environment.NewLine);
                }
            }
            //Salestb.Dispose();
            //Buystb.Dispose();
        }
    }
}
