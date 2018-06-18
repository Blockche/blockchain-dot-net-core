using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blockche.Miner.ConsoleApp.Config;
using Blockche.Miner.ConsoleApp.JobProducer;
using Blockche.Miner.ConsoleApp.Logger;

namespace Blockche.Miner.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // read config sources
            // setup env
            // setup logger

            var config = new ConsoleArgsConfigProvider(args);
            var jobProducer = new FakeJobProducer(2000, 7);
            var logger = new ConsoleLogger();

            var tokenSource = new CancellationTokenSource();
            
            var cpuMiners = new List<CpuMiner>(config.ThreadsCount);
            for (int i = 0; i < config.ThreadsCount; i++)
            {
                cpuMiners.Add(new CpuMiner(jobProducer, logger, i));
            }

            try
            {
                Task.WaitAll(cpuMiners.Select(m => Task.Factory.StartNew(
                    () => m.Start(), tokenSource.Token)).ToArray());
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
