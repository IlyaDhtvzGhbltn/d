using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RobotTraider.Logs
{
    class AppLoger
    {
        private string Path { get; set; }
        public AppLoger(string path)
        {
            this.Path = path;
        }

        public void WriteLogMessage(string message)
        {
            File.AppendAllText(this.Path, Environment.NewLine + DateTime.Now + new string(' ',5) + message);
        }
    }
}
