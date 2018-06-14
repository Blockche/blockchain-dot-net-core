using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blockche.Miner.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // read config sources
            // setup env
            // setup logger

            var tokenSource = new CancellationTokenSource();

            var threads = 4;
            var cpuMiners = new List<CpuMiner>(threads);
            for (int i = 0; i < threads; i++)
            {
                cpuMiners.Add(new CpuMiner(i));
            }

            try
            {
                Task.WaitAll(cpuMiners.Select(m => Task.Factory.StartNew(
                    () => m.Mine(tokenSource.Token), tokenSource.Token)).ToArray());
            }
            catch (AggregateException e)
            {
                foreach (var ex in e.InnerExceptions)
                {
                    Console.WriteLine(ex);
                }
            }
            finally
            {
                tokenSource.Dispose();
            }

            Console.ReadLine();
        }
    }
}
