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
        private readonly Random rnd;

        private bool isMining;
        private bool isStarted;
        private JobDTO job;

        public CpuMiner(IJobProducer jobProducer, ILogger logger, int seed)
        {
            this.jobProducer = jobProducer;
            this.logger = logger;
            this.rnd = new Random(seed);
        }

        public void Start()
        {
            this.logger.Log($"Miner start");

            this.jobProducer.JobCreated -= this.JobCreatedHandler;
            this.jobProducer.JobCreated += this.JobCreatedHandler;

            this.isStarted = true;
        }

        public void Stop()
        {
            this.logger.Log($"Miner stop");

            this.jobProducer.JobCreated -= this.JobCreatedHandler;

            this.isStarted = false;
        }

        private void JobCreatedHandler(object sender, JobCreatedEventArgs e)
        {
            if (!this.isStarted)
            {
                this.logger.Log($"Job created {e.Job.Nonce} {e.Job.Difficulty} but we are not mining");
                return;
            }

            this.logger.Log($"Job created {e.Job.Nonce} {e.Job.Difficulty}");

            this.MineBlock(e.Job).GetAwaiter().GetResult();
        }

        private async Task MineBlock(JobDTO job)
        {
            this.isMining = true;

            while (this.isMining && this.isStarted)
            {
                if (IsValidHashNonce(job))
                {
                    this.isMining = false;
                    await this.jobProducer.SubmitJob(job);
                }

                this.ChangeNonce(job);
            }
        }

        private void ChangeNonce(JobDTO job)
        {
            // TODO: Add random numbers
            job.Nonce++;
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
