using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotTraider.TradeObjects
{
    public enum C_Code
    {
        Wait = 0,
        Buy = 1,
        Sale = 2,
        Kill = 666,
        Close = 13,
        MarketBuy = 11,
        MarkefSale = 12
    }

    public enum Flag_Status
    {
        not_getting = 0,

        buy_active = 25,
        buy_confirm = 24,
        buy_kill = 26,

        confirm = 28,

        sale_active = 29,
        sale_confirm = 20,
        sale_kill = 30
    }

    public enum Trend
    {
        Bull = 1,
        Bear = -1,
        Unstable = 0
    }
}
