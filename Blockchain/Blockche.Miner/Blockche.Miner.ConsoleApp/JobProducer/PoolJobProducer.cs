using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Blockche.Miner.Common.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blockche.Miner.ConsoleApp.JobProducer
{
    public class PoolJobProducer : IJobProducer
    {
        private readonly string user;
        private readonly string worker;

        private readonly HubConnection connection;

        public PoolJobProducer(IEnumerable<string> poolAddresses, string user, string worker)
        {
            this.user = user;
            this.worker = worker;

            foreach (var poolAddress in poolAddresses)
            {
                this.connection = new HubConnectionBuilder()
                    .WithUrl(poolAddress + "/poolhub", h => 
                    {
                        h.Headers.Add("X-USER", this.user);
                        h.Headers.Add("X-WORKER", this.worker);
                    })
                    .Build();

                this.connection.On<JobDTO>("NewJob", j => JobCreated?.Invoke(this, new JobCreatedEventArgs { Job = j }));
                
                this.connection.StartAsync().GetAwaiter().GetResult();

                break;
            }
        }

        public event EventHandler<JobCreatedEventArgs> JobCreated;

        public Task ReportHashrate(decimal hashRate)
        {
            return this.connection.InvokeAsync("ReportHashrate", hashRate);
        }

        public Task SubmitJob(JobDTO job)
        {
            job.User = this.user;
            job.Worker = this.worker;
            return this.connection.InvokeAsync("SubmitJob", job);
        }
    }
}
