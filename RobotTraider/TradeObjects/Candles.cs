using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeObjects
{

    public class Candle
    {
        public float open { get; set; }
        public float close { get; set; }
        public float high { get; set; }
        public float low { get; set; }
        public int volume { get; set; }
        public int index { get; set; }
    }
}
