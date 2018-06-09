using RobotTraider.QuikDataObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotTraider.Strategists
{
    interface Istrategies
    {
        ToQuikCommand[] GetSolution(object glas);
    }
}
