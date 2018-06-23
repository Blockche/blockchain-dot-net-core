using Blockche.Miner.Common.Models;
using Blockche.Miner.PoolWebApp.Mining;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockche.Miner.PoolWebApp.Models
{
    public class DashboardModel
    {
        public List<MinerOverview> TopMiners { get; set; }

        public MinersReport Report { get; set; }

        public List<JobDTO> MinedJobs { get; set; }
    }
}
