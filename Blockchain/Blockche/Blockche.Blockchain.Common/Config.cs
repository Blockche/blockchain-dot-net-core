﻿using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.Text;


namespace Blockche.Blockchain.Common
{
    public class Config
    {
        public const int StartDifficulty = 3;
        public const int MinTransactionFee = 10;
        public const int MaxTransactionFee = 1000000;
        public const int BlockReward = 5000000;
        public const long MaxTransferValue = 10000000000000;
        public const int SafeConfirmCount = 3;

        public const int TargetSecondsBetweenBlocks = 30;
        public const double AllowedOffsetPercentage = 20;
        public const double MinTargetSecondsBetweenBlocks = TargetSecondsBetweenBlocks * (1 - (AllowedOffsetPercentage / 100));
        public const double MaxTargetSecondsBetweenBlocks = TargetSecondsBetweenBlocks * (1 + (AllowedOffsetPercentage / 100));

        public const string NullAddress = "0000000000000000000000000000000000000000";
        public const string NullPubKey = "00000000000000000000000000000000000000000000000000000000000000000";                                          

        public const string NullSignaturePublicKey = "0000000000000000000000000000000000000000000000000000000000000000";
        public const string NullSignaturePrivateKey = NullSignaturePublicKey;
        public static readonly int FaucetDailyAddressRequestCountMax = 2;
        public static readonly int FaucetWaitMinutes = 1;

        public static string[] GetNullSignatureHex()
        {
            string[] sig = new string[2];
            sig[0] = NullSignaturePublicKey;
            sig[1] = NullSignaturePrivateKey;

            return sig;
        }

        public static BigInteger[] GetNullSignature()
        {
            BigInteger[] sig = new BigInteger[2];
            sig[0] = new BigInteger(NullSignaturePublicKey, 16);
            sig[1] = new BigInteger(NullSignaturePrivateKey, 16);

            return sig;
        }
    }
}
