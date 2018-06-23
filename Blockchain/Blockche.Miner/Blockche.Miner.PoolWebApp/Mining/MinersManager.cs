using Blockche.Miner.Common.Models;
using Blockche.Miner.PoolWebApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Blockche.Miner.PoolWebApp.Mining
{
    public static class MinersManager
    {
        private static readonly List<JobDTO> minedJobs = new List<JobDTO>();
        private static readonly Dictionary<string, string> connectionsMiners = new Dictionary<string, string>();
        private static readonly Dictionary<string, Miner> miners = new Dictionary<string, Miner>();
        private static readonly Dictionary<string, HashSet<string>> userWorkers = new Dictionary<string, HashSet<string>>();
        private static readonly Random rnd = new Random();

        public static int LastDifficulty { get; set; }

        public static JobDTO LastJob { get; set; }

        public static void AddMinedBlock(JobDTO job)
        {
            minedJobs.Add(job);
        }

        public static void AddMiner(string user, string worker)
        {
            if (string.IsNullOrEmpty(user))
            {
                return;
            }

            if (userWorkers.ContainsKey(user))
            {
                userWorkers[user].Add(worker);
            }
            else
            {
                userWorkers.Add(user, new HashSet<string> { worker });
            }

            var key = $"{user}.{worker}";

            miners[key] = new Miner
            {
                User = user,
                Worker = worker
            };
        }

        public static void RemoveMiner(string user, string worker)
        {
            if (string.IsNullOrEmpty(user))
            {
                return;
            }

            if (userWorkers.ContainsKey(user))
            {
                userWorkers[user].Remove(worker);
            }

            var key = $"{user}.{worker}";

            if (miners.ContainsKey(key))
            {
                miners.Remove(key);
            }
        }

        public static void ReportHashrate(string user, string worker, decimal hashrate)
        {
            var key = $"{user}.{worker}";
            if (miners.ContainsKey(key))
            {
                miners[key].Hashrate = hashrate;
            }
        }

        public static List<JobDTO> GetLastMinedJobs()
        {
            return minedJobs.TakeLast(10).ToList();
        }

        public static MinersReport GetReport()
        {
            return new MinersReport
            {
                Hashrate = miners.Values.Sum(m => m.Hashrate),
                LastDifficulty = LastDifficulty,
                Miners = userWorkers.Keys.Count,
                Workers = miners.Values.Count
            };
        }

        public static List<MinerOverview> GetTopMiners()
        {
            return miners.Values
                .GroupBy(m => m.User)
                .Select(gr => new MinerOverview { User = gr.Key, Hashrate = gr.Sum(g => g.Hashrate), WorkersCount = gr.Count() })
                .OrderByDescending(m => m.Hashrate)
                .Take(10)
                .ToList();
        }
    }
}
