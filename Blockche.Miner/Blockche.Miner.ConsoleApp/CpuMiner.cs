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
            if (!this.isStarted)
            {
                //this.logger.Log($"[{this.seed}] Job created {e.Job.Difficulty} but we are not mining");
                return;
            }

            //this.logger.Log($"[{this.seed}] Job created {e.Job.Difficulty}");

            this.MineBlock(e.Job).GetAwaiter().GetResult();
        }

        private async Task MineBlock(JobDTO job)
        {
            this.isMining = true;

            // Initially set to random nonce
            this.RandomizeNonce(job);

            while (this.isMining && this.isStarted)
            {

                if (IsValidHashNonce(job))
                {
                    this.isMining = false;
                    await this.jobProducer.SubmitJob(job, this.seed);
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
                job.Nonce = BitConverter.ToInt64(this.buffer, 0);
            }
        }

        private void RandomizeNonce(JobDTO job)
        {
            do
            {
                rnd.NextBytes(this.buffer);
                job.Nonce = BitConverter.ToInt64(this.buffer, 0);
            }
            while (job.Nonce < 0);
        }

        private bool IsValidHashNonce(JobDTO job)
        {
            var hash = HashHelper.ComputeSHA256($"{job.TxHash}{job.Nonce}");
            for (int i = 0; i < job.Difficulty; i++)
            {
                if (hash[i] != '0')
                {
                    return false;
                }
            }

            return true;
        }
    }
}
