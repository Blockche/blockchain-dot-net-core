using System;
using System.Threading.Tasks;

namespace Blockche.Miner.ConsoleApp.JobProducer
{
    public interface IJobProducer
    {
        event EventHandler<JobCreatedEventArgs> JobCreated;

        Task SubmitJob(JobDTO job, int miner);
    }
}
