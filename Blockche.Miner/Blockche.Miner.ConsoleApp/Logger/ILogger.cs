using System;
using System.Collections.Generic;
using System.Text;

namespace Blockche.Miner.ConsoleApp.Logger
{
    public interface ILogger
    {
        void Log(string message);

        void Error(string error);

        void Error(Exception ex);
    }
}
