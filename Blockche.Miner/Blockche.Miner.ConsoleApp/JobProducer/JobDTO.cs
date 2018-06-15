using System;
using System.Collections.Generic;
using System.Text;

namespace Blockche.Miner.ConsoleApp.JobProducer
{
    public class JobDTO
    {
        public long Nonce { get; set; }

        public int Difficulty { get; set; }

        public string TxHash { get; set; }
    }
}
