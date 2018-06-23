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
        private static readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public static void Main(string[] args)
        {
            var config = new ConsoleArgsConfigProvider();

            IJobProducer jobProducer;
            if (config.IsTest)
            {
                jobProducer = new FakeJobProducer(2000, 7);
            }
            else if (config.UsePool)
            {
                jobProducer = new PoolJobProducer(config.JobProducerUrls, config.User, config.Worker);
            }
            else
            {
                jobProducer = new HttpJobProducer(config.Address, config.JobProducerUrls);
            }

            var logger = new ConsoleLogger();
            
            var cpuMiners = new List<CpuMiner>(config.ThreadsCount);
            for (int i = 0; i < config.ThreadsCount; i++)
            {
                cpuMiners.Add(new CpuMiner(jobProducer, logger, i));
            }

            try
            {
                Parallel.ForEach(cpuMiners, miner => miner.Start());
            }
            catch (AggregateException e)
            {
                foreach (var ex in e.InnerExceptions)
                {
                    Console.WriteLine(ex);
                }

                cancellationTokenSource.Dispose();
            }

            Console.CancelKeyPress += Console_CancelKeyPress;
            Console.ReadLine();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
    }
}
