using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charts
{
    public class chartData
    {
        public float Hoog;
        public float Laag;
        public float PrijsOpen;
        public float PrijsGesloten;

        public chartData( float h, float l, float o, float c)
        {
            Hoog = h; Laag = l; PrijsOpen = o; PrijsGesloten = c;
        }
    }
}
