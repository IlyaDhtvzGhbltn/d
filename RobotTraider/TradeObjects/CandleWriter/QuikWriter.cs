using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TradeObjects.QuikDataObj;
using Newtonsoft.Json;
using System.IO;

namespace TradeObjects.CandleWriter
{
    public class QuikWriter
    {
        int index = -1;
        string path;

        public QuikWriter()
        {
            path = string.Format("Candles.{0}.json", DateTime.Now.ToString("yyyy.MM.dd.hh.mm"));
        }

        public QuikWriter(string path)
        {
            this.path = path;
        }

        public ToQuikCommand WriteCandles(Candle[] candleSet)
        {

            if (candleSet != null)
            {
                WriteFile(candleSet);
            }
            return null;
        }



        private void WriteFile(Candle[] candleSet)
        {

            string[] jsonLines = new string[1];

            if (index == -1)
            {
                jsonLines[0] = JsonConvert.SerializeObject(candleSet[0]);
                File.AppendAllLines(path, jsonLines);

            }
            else
            {
                if (index != candleSet[0].index)
                {
                    jsonLines[0] = JsonConvert.SerializeObject(candleSet[0]);
                    File.AppendAllLines(path, jsonLines);

                }
            }
            index = candleSet[0].index;
        }


    }
}
