using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Charts
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            List<chartData> dd = new List<chartData>()
            {
                new chartData(14,7,9,10),
                new chartData(17,1,1,12),
                new chartData(14,5,4,13)
            };


            Series price = new Series("price");
            price.Color = Color.Red;
            chart1.Series.Add(price);

            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;


            chart1.Series["price"].ChartType = SeriesChartType.Candlestick;

            for (int i =0; i< dd.Count; i++)
            {
                chart1.Series["price"].Points.AddXY(i, dd[i].Hoog);

                chart1.Series["price"].Points[i].YValues[1] = dd[i].Laag;
                //adding open
                chart1.Series["price"].Points[i].YValues[2] = dd[i].PrijsOpen;
                // adding close
                chart1.Series["price"].Points[i].YValues[3] = dd[i].PrijsGesloten;
            }

            chart1.SaveImage("chart.jpeg", ChartImageFormat.Jpeg);
        }
    }
}
