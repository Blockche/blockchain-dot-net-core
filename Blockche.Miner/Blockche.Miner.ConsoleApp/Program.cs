using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

            var jobProducer = new FakeJobProducer();
            var logger = new ConsoleLogger();

            var tokenSource = new CancellationTokenSource();

            var threads = 1;
            var cpuMiners = new List<CpuMiner>(threads);
            for (int i = 0; i < threads; i++)
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

    public class FakeJobProducer : IJobProducer
    {
        public event EventHandler<JobCreatedEventArgs> JobCreated;

        private readonly System.Timers.Timer timer;

        public FakeJobProducer()
        {
            this.timer = new System.Timers.Timer(2000);
            this.timer.Elapsed += this.Timer_Elapsed;
            this.timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.JobCreated.Invoke(this, new JobCreatedEventArgs { Job = new JobDTO { Difficulty = 4, TxHash = Guid.NewGuid().ToString("N") } });
        }

        public Task SubmitJob(JobDTO job)
        {
            Console.WriteLine("Job submited!");
            this.JobCreated.Invoke(this, new JobCreatedEventArgs { Job = new JobDTO { Difficulty = 4, TxHash = Guid.NewGuid().ToString("N") } });
            return Task.CompletedTask;
        }
    }

    public class ConsoleLogger : ILogger
    {
        public void Error(string error)
        {
            Console.WriteLine(error);
        }

        public void Error(Exception ex)
        {
            Console.WriteLine(ex);
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
