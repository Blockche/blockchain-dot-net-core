using Blockche.Miner.Common.Models;
using Blockche.Miner.PoolWebApp.Mining;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockche.Miner.PoolWebApp.Hubs
{
    public class PoolHub : Hub
    {
        private readonly JobProducer jobProducer;

        public PoolHub(JobProducer jobProducer)
        {
            this.jobProducer = jobProducer;
        }

        public Task SubmitJob(JobDTO job)
        {
            return this.jobProducer.SubmitJob(job);
        }

        public Task ReportHashrate(decimal rate)
        {
            MinersManager.ReportHashrate(this.XUser, this.XWorker, rate);
            return Task.CompletedTask;
        }

        public override Task OnConnectedAsync()
        {
            MinersManager.AddMiner(this.XUser, this.XWorker);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            MinersManager.RemoveMiner(this.XUser, this.XWorker);
            return base.OnDisconnectedAsync(exception);
        }

        private string XUser
        {
            get
            {
                var req = this.Context.GetHttpContext().Request;
                var user = req.Headers["X-USER"];
                if (string.IsNullOrEmpty(user))
                {
                    user = req.Query["xuser"];
                }

                return user;
            }
        }

        private string XWorker
        {
            get
            {
                var req = this.Context.GetHttpContext().Request;
                var worker = req.Headers["X-WORKER"];
                if (string.IsNullOrEmpty(worker))
                {
                    worker = req.Query["xworker"];
                }

                return worker;
            }
        }
    }
}
