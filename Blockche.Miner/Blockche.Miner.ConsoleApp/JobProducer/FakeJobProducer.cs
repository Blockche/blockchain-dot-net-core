using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blockche.Miner.ConsoleApp.JobProducer
{
    public class FakeJobProducer : IJobProducer
    {
        public event EventHandler<JobCreatedEventArgs> JobCreated;

        private readonly System.Timers.Timer timer;
        private readonly int interval;
        private readonly int difficulty;

        public FakeJobProducer(int interval, int difficulty)
        {
            this.timer = new System.Timers.Timer(interval);
            this.timer.Elapsed += this.Timer_Elapsed;
            this.timer.Start();
            this.interval = interval;
            this.difficulty = difficulty;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine($"Times up - new Job! diff: {this.difficulty}");
            this.JobCreated.Invoke(this, this.BuildJobCreatedArgs());
        }

        public Task SubmitJob(JobDTO job, int miner)
        {
            Console.WriteLine($"Job submited! {job.Nonce} by {miner}");
            this.timer.Interval = this.interval;
            this.JobCreated.Invoke(this, this.BuildJobCreatedArgs());
            return Task.CompletedTask;
        }

        private JobCreatedEventArgs BuildJobCreatedArgs()
            => new JobCreatedEventArgs { Job = new JobDTO { Difficulty = this.difficulty, TxHash = Guid.NewGuid().ToString("N") } };
    }
}
