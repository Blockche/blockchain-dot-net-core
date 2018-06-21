using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockche.Blockchain.Web.Models
{
    public class SubmitMinedBlockRequestModel
    {
        public ulong Nonce { get; set; }

        // Hex
        public string BlockHash { get; set; }

        // Hex
        public string BlockDataHash { get; set; }

        public string DateCreated { get; set; }
    }
}
