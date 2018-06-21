using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blockche.Miner.ConsoleApp.JobProducer
{
    public class PoolJobProducer : IJobProducer
    {
        public event EventHandler<JobCreatedEventArgs> JobCreated;

        public Task SubmitJob(JobDTO job, int miner) => throw new NotImplementedException();
    }
}
