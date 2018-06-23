using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockche.Miner.PoolWebApp.Mining
{
    public class MinerOverview
    {
        public string User { get; set; }

        public int WorkersCount { get; set; }

        public decimal Hashrate { get; set; }
    }
}
