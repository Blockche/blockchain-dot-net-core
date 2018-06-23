using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockche.Miner.PoolWebApp.Mining
{
    public class MinersReport
    {
        public decimal Hashrate { get; set; }

        public decimal LastDifficulty { get; set; }

        public int Miners { get; set; }

        public int Workers { get; set; }
    }
}
