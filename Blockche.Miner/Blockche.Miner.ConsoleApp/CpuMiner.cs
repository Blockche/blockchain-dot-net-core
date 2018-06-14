using Blockche.Miner.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blockche.Miner.ConsoleApp
{
    public class CpuMiner
    {
        private readonly int instanceNonce;
        private readonly CancellationTokenSource tokenSource;

        private int difficulty;
        private string prevBlockHash;

        public CpuMiner(int instanceNonce)
        {
            this.instanceNonce = instanceNonce;
            this.tokenSource = new CancellationTokenSource();
        }

        public async Task Mine(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var res = await MineBlock(this.tokenSource.Token, 0, "asd", "41", 2);
            }
        }

        public Task<long> MineBlock(CancellationToken cancellationToken, long nonce, string blockDataHash, string prevBlockHash, int difficulty)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (IsValidHashNonce(++nonce, blockDataHash, prevBlockHash, difficulty))
                {
                    Console.WriteLine($"{this.instanceNonce} Found block!");
                    return Task.FromResult(nonce);
                }
            }

            return Task.FromCanceled<long>(cancellationToken);
        }

        private bool IsValidHashNonce(long nonce, string blockDataHash, string prevBlockHash, int difficulty)
        {
            var hash = HashHelper.ComputeSHA256($"{prevBlockHash}{blockDataHash}{nonce}");
            for (int i = 0; i < difficulty; i++)
            {
                if (hash[i] != '0')
                {
                    Console.WriteLine($"{this.instanceNonce} Checking hash ... invalid");
                    return false;
                }
            }

            Console.WriteLine($"{this.instanceNonce} Checking hash ... valid");
            return true;
        }
    }
}
