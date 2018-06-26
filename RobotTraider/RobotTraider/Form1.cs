using System;
using System.Threading;
using DataContractRializer;
using System.Windows.Forms;
using TradingStrategistQUIK;
using static TradeObjects.QuikDataObj;
using Istrategies = TradeObjects.Strategies.Istrategies;
using Connector = TradeObjects.LuaConnection.LuaMMFConnector;
using static TradingStrategistQUIK.SBER_actio;

namespace RobotTraider
{
    public partial class Form1 : Form
    {
        // Ключевое слово "volatile" гарантирует, что в переменной в любое время и при обращении из любого потока будет актуальное значение
        public volatile bool Run = true;
        Connector CommandCtor;
        Connector GlassCtor;
        delegate void Print(string glasscellprice);


        ScalpNewVer Scalp;
        public Form1()
        {
            InitializeComponent();
            GlassCtor = new Connector("TerminalQuote");
            CommandCtor = new Connector("QUIKCommand");
            Scalp = new SBER_actio.ScalpNewVer("QJSIM", "SBER", "NL0011100043", "Orders");
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


                    FillVisual(glass, Strateg);
                    ToQuikCommand BidComms = Strateg.GetToQUIKCommand(glass);
                    var LuaCommand = Serializer.QuikCommandToTransaction(BidComms);
                    CommandCtor.SendData(LuaCommand);
                    Thread.Sleep(1);
                    
                }
            })
            .Start();
        }

        private void FillVisual(Glass glass, Istrategies str)
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
                var midd = glass.MiddleQuantity();
                Invoke(new Print((s) => label3.Text = s), midd.ToString());
                Invoke(new Print((s) => label4.Text = s), glass.GetBidQuantMoreMidd(midd, 2).Item2.ToString());
                Invoke(new Print((s) => label7.Text = s), glass.GetBidQuantMoreMidd(midd, 2).Item3.ToString());
                Invoke(new Print((s) => label11.Text = s), glass.GetBidQuantMoreMidd(midd, 2).Item1.ToString());
                Invoke(new Print((s) => label15.Text = s), glass.getActualPrice().ToString());
                if(((ScalpNewVer)str).MyPosition != null)
                Invoke(new Print((s) => label14.Text = s), ((ScalpNewVer)str).MyPosition.Price.ToString());


            }
        }
    }
}
