using Blockche.Miner.Common;
using Blockche.Miner.Common.Models;
using Blockche.Miner.ConsoleApp.JobProducer;
using Blockche.Miner.ConsoleApp.Logger;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blockche.Miner.ConsoleApp
{
    public class CpuMiner
    {
        private readonly IJobProducer jobProducer;
        private readonly ILogger logger;
        private readonly int seed;
        private readonly Random rnd;
        private readonly byte[] buffer;
        private readonly System.Timers.Timer hashRateReportTimer;

        private bool isMining;
        private bool isStarted;

        private int operationsCount = 0;
        private DateTime operationsStart = DateTime.UtcNow;
        private decimal hashRate = 0;

        public CpuMiner(IJobProducer jobProducer, ILogger logger, int seed)
        {
            this.jobProducer = jobProducer;
            this.logger = logger;
            this.seed = seed;
            this.rnd = new Random(seed);
            this.buffer = new byte[8];

            this.hashRateReportTimer = new System.Timers.Timer(2000);
            this.hashRateReportTimer.Elapsed += (s, e) =>
            {
                if (this.hashRate > 0)
                {
                    this.logger.Log($"[{this.seed}] Hashrate -> " + this.hashRate);
                    this.jobProducer.ReportHashrate(this.hashRate).GetAwaiter().GetResult();
                }
            };
            this.hashRateReportTimer.Start();
        }

        public void Start()
        {
            this.logger.Log($"[{this.seed}] Miner start");

            this.isStarted = true;

            this.jobProducer.JobCreated -= this.JobCreatedHandler;
            this.jobProducer.JobCreated += this.JobCreatedHandler;

            var job = this.jobProducer.GetJob().GetAwaiter().GetResult();
            if (job != null)
            {
                this.JobCreatedHandler(null, new JobCreatedEventArgs { Job = job });
            }
        }

        public void Stop()
        {
            this.logger.Log($"[{this.seed}] Miner stop");

            this.jobProducer.JobCreated -= this.JobCreatedHandler;

            this.isStarted = false;
        }

        private void JobCreatedHandler(object sender, JobCreatedEventArgs e)
        {
            this.logger.Log($"[{this.seed}] New job arrived {e.Job.Difficulty} {e.Job.BlockDataHash}");
            if (!this.isStarted)
            {
                return;
            }

            this.isMining = false;

            // Waiting for cycle to complete
            Thread.Sleep(2000); 
            this.MineBlock(e.Job).GetAwaiter().GetResult();
        }

        private async Task MineBlock(JobDTO job)
        {
            this.isMining = true;

            // Initially set to random nonce
            this.RandomizeNonce(job);
            job.DateCreated = DateTimeHelper.UtcNowToISO8601();

            this.operationsCount = 0;
            this.operationsStart = DateTime.UtcNow;

            while (this.isMining && this.isStarted)
            {
                this.operationsCount++;
                if (IsValidHashNonce(job))
                {
                    this.UpdateHashrate();

                    this.isMining = false;
                    this.logger.Log($"[{this.seed}] Submiting mined block {job.BlockHash}");
                    await this.jobProducer.SubmitJob(job);
                    break;
                }

                this.GetNextNonce(job);
            }
        }

        private void UpdateHashrate()
        {
            try
            {
                if (this.operationsCount > 0)
                {
                    this.hashRate = (decimal)this.operationsCount / (DateTime.UtcNow - this.operationsStart).Milliseconds;
                }
            }
            catch (DivideByZeroException)
            {
            }

            this.operationsCount = 0;
            this.operationsStart = DateTime.UtcNow;
        }

        private void GetNextNonce(JobDTO job)
        {
            unchecked
            {
                job.Nonce++;
            }

            while (job.Nonce < 0)
            {
                rnd.NextBytes(this.buffer);
                job.Nonce = BitConverter.ToUInt64(this.buffer, 0);
            }
        }

        private void RandomizeNonce(JobDTO job)
        {
            do
            {
                rnd.NextBytes(this.buffer);
                job.Nonce = BitConverter.ToUInt64(this.buffer, 0);
            }
            while (job.Nonce < 0);
        }

        private bool IsValidHashNonce(JobDTO job)
        {
            job.BlockHash = HashHelper.ComputeSHA256($"{job.BlockDataHash}|{job.DateCreated}|{job.Nonce}");
            for (int i = 0; i < job.Difficulty; i++)
            {
                if (job.BlockHash[i] != '0')
                {
                    return false;
                }
            }

            return true;
        }
    }
}
