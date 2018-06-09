using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RobotTraider.LuaStrConverter;
using RobotTraider.QuikDataObj;
using RobotTraider.Strategists;
using RobotTraider.Strategists.Scalping;
using Connector = RobotTraider.LuaConnection.LuaMMFConnector;

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

            Scalp = new GlassScalping("TQBR", "SBER", "L01-00000F00", "Orders");

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

                    ToQuikCommand[] BidComms = Strateg.GetSolution(glass);
                    if (BidComms!=null && BidComms.Length>0)
                    for (int i =0; i<BidComms.Length; i++)
                    {
                        if (BidComms[i] != null)
                        {
                            var LuaCommand = Serializer.QuikCommandToTransaction(BidComms[i]);
                            CommandCtor.SendData(LuaCommand);
                        }
                        Thread.Sleep(1);
                    }
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
