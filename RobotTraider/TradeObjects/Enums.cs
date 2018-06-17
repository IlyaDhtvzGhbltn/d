using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeObjects
{
    public class Enums
    {
        public enum Operation
        {
            Wait = 0,
            Buy = 1,
            Sale = 2,
            Cancel = 666,
            CloseLong = 11,
            CloseShort = 12
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

        public enum PositionStatus
        {
            notPosition,
            SendBidLong,
            SendBidShort,
            OpenPositionLONG,
            OpenPositionSHORT
       }
    }
}
