using System;
using System.Collections.Generic;
using System.Text;

namespace Blockche.Miner.ConsoleApp.Config
{
    public class ConsoleArgsConfigProvider : IConfigProvider
    {
        public ConsoleArgsConfigProvider(string[] args)
        {

        }

        public int ThreadsCount => throw new NotImplementedException();

        public string JobProducerUri => throw new NotImplementedException();

        public bool UsePool => throw new NotImplementedException();
    }
}
