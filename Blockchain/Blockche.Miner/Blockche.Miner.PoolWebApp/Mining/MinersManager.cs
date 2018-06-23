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
        private static readonly Dictionary<string, string> connectionsMiners = new Dictionary<string, string>();
        private static readonly Dictionary<string, Miner> miners = new Dictionary<string, Miner>();
        private static readonly Dictionary<string, HashSet<string>> userWorkers = new Dictionary<string, HashSet<string>>();
        private static readonly Random rnd = new Random();

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
    }
}
