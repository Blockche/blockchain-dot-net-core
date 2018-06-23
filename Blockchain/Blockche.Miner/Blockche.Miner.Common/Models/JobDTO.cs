using System;
using System.Collections.Generic;
using System.Text;

namespace Blockche.Miner.Common.Models
{
    public class JobDTO
    {
        public int Difficulty { get; set; }

        public ulong Nonce { get; set; }

        public string DateCreated { get; set; }

        public string BlockHash { get; set; }

        public string BlockDataHash { get; set; }

        public string User { get; set; }

        public string Worker { get; set; }
    }
}
