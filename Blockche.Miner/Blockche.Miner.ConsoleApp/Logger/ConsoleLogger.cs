using System;
using System.Collections.Generic;
using System.Text;

namespace Blockche.Miner.ConsoleApp.Logger
{
    public class ConsoleLogger : ILogger
    {
        public void Error(string error)
        {
            Console.WriteLine(error);
        }

        public void Error(Exception ex)
        {
            Console.WriteLine(ex);
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
