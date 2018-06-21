using Blockche.Miner.Common;
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

        private bool isMining;
        private bool isStarted;

        public CpuMiner(IJobProducer jobProducer, ILogger logger, int seed)
        {
            this.jobProducer = jobProducer;
            this.logger = logger;
            this.seed = seed;
            this.rnd = new Random(seed);
            this.buffer = new byte[8];
        }

        public void Start()
        {
            this.logger.Log($"[{this.seed}] Miner start");

            this.jobProducer.JobCreated -= this.JobCreatedHandler;
            this.jobProducer.JobCreated += this.JobCreatedHandler;

            this.isStarted = true;
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

            while (this.isMining && this.isStarted)
            {
                if (IsValidHashNonce(job))
                {
                    this.isMining = false;
                    this.logger.Log($"[{this.seed}] Submiting mined block {job.BlockHash}");
                    await this.jobProducer.SubmitJob(job, this.seed);
                    break;
                }

                this.GetNextNonce(job);
            }
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
