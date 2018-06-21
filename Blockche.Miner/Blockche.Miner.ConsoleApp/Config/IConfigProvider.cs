using System;
using System.Collections.Generic;
using System.Text;

namespace Blockche.Miner.ConsoleApp.Config
{
    public interface IConfigProvider
    {
        int ThreadsCount { get; }

        IEnumerable<string> JobProducerUrls { get; }

        bool UsePool { get; }
    }
}
