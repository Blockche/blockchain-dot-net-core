using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;

namespace Blockche.Miner.ConsoleApp.JobProducer
{
    public class HttpJobProducer : IJobProducer
    {
        private readonly HttpClient http;
        private readonly string minerAddress;
        private readonly IEnumerable<string> nodeUrls;
        private readonly Timer timer;
        private const int TimerInterval = 1000 * 5;

        private string lastJob;

        public HttpJobProducer(string minerAddress, IEnumerable<string> nodeUrls)
        {
            this.http = new HttpClient();
            this.minerAddress = minerAddress;
            this.nodeUrls = nodeUrls;

            // Query every 5sec for new job
            this.timer = new Timer(TimerInterval);
            this.timer.Elapsed += this.OnTimerElapsed;
            this.timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var nodeUrl in this.nodeUrls)
            {
                var newJob = this.GetJob(nodeUrl).GetAwaiter().GetResult();
                if (newJob != null)
                {
                    this.NotifyNewJob(newJob);
                    break;
                }
            }
        }

        public event EventHandler<JobCreatedEventArgs> JobCreated;

        public async Task SubmitJob(JobDTO job, int miner)
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

            foreach (var nodeUrl in this.nodeUrls)
            {
                var fullUrl = $"{nodeUrl}/api/node/mining/submit-mined-block";
                var response = await this.http.PostAsync(fullUrl, payload);
                if (response.IsSuccessStatusCode)
                {
                    this.timer.Interval = TimerInterval;
                    this.OnTimerElapsed(null, null);
                    break;
                }
            }
        }
        
        private void NotifyNewJob(JobDTO newJob)
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
            this.JobCreated?.Invoke(this, new JobCreatedEventArgs { Job = newJob });
        }

        private async Task<JobDTO> GetJob(string nodeUrl)
        {
            // Get the next mining job to invoke event
            var newJobResponse = await this.http.GetAsync($"{nodeUrl}/api/node/mining/get-mining-job/{this.minerAddress}");
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
    }
}
