using System;
using System.Collections.Generic;
using System.Text;

namespace Blockche.Blockchain.Common
{
    public class Config
    {
        public const int StartDifficulty = 5;
        public const int MinTransactionFee = 10;
        public const int MaxTransactionFee = 1000000;
        public const int BlockReward = 5000000;
        public const long MaxTransferValue = 10000000000000;
        public const int SafeConfirmCount = 3;
    }
}
