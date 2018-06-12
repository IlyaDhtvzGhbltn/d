using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QUIKException
{
    public class TermException : Exception
    {
        public override string Message => "Unexpected Conditions";
    }

    public class ListOrderException : Exception
    {
        public override string Message => "List Orders is Empty";
    }
}
