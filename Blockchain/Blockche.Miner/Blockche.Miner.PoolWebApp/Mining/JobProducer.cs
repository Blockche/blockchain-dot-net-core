using Blockche.Miner.Common.Models;
using Blockche.Miner.PoolWebApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Blockche.Miner.PoolWebApp.Mining
{
    public class JobProducer
    {
        private const string Address = "1234567890123456789012345678901234567890";
        private const string NodeAddress = "http://localhost:59415";

        private const string SubmitUrlFormat = "{0}/api/node/mining/submit-mined-block";
        private const string GetMiningJobFormat = "{0}/api/node/mining/get-mining-job/{1}";

        private const int NewJobCheckerInterval = 5000;

        private readonly HttpClient http;
        private readonly Timer newJobChecker;
        private readonly string address;
        private readonly string[] nodes;
        private readonly IServiceProvider serviceProvider;

        private string lastJob;

        public JobProducer(IServiceProvider serviceProvider)
        {
            this.http = new HttpClient();
            this.address = Address;
            this.nodes = new[] { NodeAddress };

            this.newJobChecker = new Timer(NewJobCheckerInterval);
            this.newJobChecker.Elapsed += this.NewJobChecker_Elapsed;
            this.newJobChecker.Start();
            this.serviceProvider = serviceProvider;
        }

        private void NewJobChecker_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                OnNewJobCheckerIntervalAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
            }
        }

        public async Task SubmitJob(JobDTO job)
        {
            var minedBlock = new MinedBlock
            {
                BlockDataHash = job.BlockDataHash,
                BlockHash = job.BlockHash,
                DateCreated = job.DateCreated,
                Nonce = job.Nonce
            };

            var payload = new ByteArrayContent(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(minedBlock)));
            payload.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            foreach (var node in this.nodes)
            {
                var fullUrl = $"{node}/api/node/mining/submit-mined-block";
                var response = await this.http.PostAsync(fullUrl, payload);
                if (response.IsSuccessStatusCode)
                {
                    MinersManager.AddMinedBlock(job);
                    this.newJobChecker.Interval = NewJobCheckerInterval;
                    await this.OnNewJobCheckerIntervalAsync();
                    break;
                }
            }
        }

        private async Task OnNewJobCheckerIntervalAsync()
        {
            foreach (var node in this.nodes)
            {
                var newJob = this.GetJob(node).GetAwaiter().GetResult();
                if (newJob != null)
                {
                    await this.NotifyNewJob(newJob);
                    break;
                }
            }
        }

        private async Task<JobDTO> GetJob(string nodeUrl)
        {
            // Get the next mining job to invoke event
            var newJobResponse = await this.http.GetAsync($"{nodeUrl}/api/node/mining/get-mining-job/{this.address}");
            if (newJobResponse.IsSuccessStatusCode)
            {
                var content = await newJobResponse.Content.ReadAsStringAsync();
                var newBlock = JsonConvert.DeserializeObject<NewBlock>(content);
                return new JobDTO
                {
                    BlockDataHash = newBlock.BlockDataHash,
                    Difficulty = newBlock.Difficulty
                };
            }

            return null;
        }

        private async Task NotifyNewJob(JobDTO newJob)
        {
            if (newJob == null)
            {
                return;
            }

            var serializedNewJob = JsonConvert.SerializeObject(newJob);
            if (serializedNewJob == this.lastJob)
            {
                return;
            }

            this.lastJob = serializedNewJob;

            MinersManager.LastDifficulty = newJob.Difficulty;
            MinersManager.LastJob = newJob;

            var ctx = this.serviceProvider.GetRequiredService<IHubContext<PoolHub>>();
            await ctx.Clients.All.SendAsync("NewJob", newJob);
        }
    }
}
