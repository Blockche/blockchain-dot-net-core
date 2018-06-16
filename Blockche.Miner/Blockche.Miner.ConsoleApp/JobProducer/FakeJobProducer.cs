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
        private readonly int difficulty;

        public FakeJobProducer(int interval, int difficulty)
        {
            this.timer = new System.Timers.Timer(interval);
            this.timer.Elapsed += this.Timer_Elapsed;
            this.timer.Start();
            this.difficulty = difficulty;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Times up - new Job!");
            this.JobCreated.Invoke(this, this.BuildJobCreatedArgs());
        }

        public Task SubmitJob(JobDTO job)
        {
            Console.WriteLine("Job submited!");
            this.timer.Stop();
            this.JobCreated.Invoke(this, this.BuildJobCreatedArgs());
            this.timer.Start();
            return Task.CompletedTask;
        }

        private JobCreatedEventArgs BuildJobCreatedArgs()
            => new JobCreatedEventArgs { Job = new JobDTO { Difficulty = this.difficulty, TxHash = Guid.NewGuid().ToString("N") } };
    }
}
