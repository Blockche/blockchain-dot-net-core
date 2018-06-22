namespace Blockche.Miner.Common.Models
{
    public class MinedBlock
    {
        public ulong Nonce { get; set; }

        public string BlockHash { get; set; }

        public string BlockDataHash { get; set; }

        public string DateCreated { get; set; }
    }
}
