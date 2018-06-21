using System;
using System.Collections.Generic;
using System.Text;

namespace Blockche.Miner.ConsoleApp.JobProducer
{
    public class JobCreatedEventArgs : EventArgs
    {
        public JobDTO Job { get; set; }
    }
}
