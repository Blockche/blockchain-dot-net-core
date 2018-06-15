using Org.BouncyCastle.Math;
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

        public const string NullAddress = "0000000000000000000000000000000000000000";
        public const string NullPubKey = "00000000000000000000000000000000000000000000000000000000000000000";
                                          

        public const string NullSignaturePublicKey = "0000000000000000000000000000000000000000000000000000000000000000";
        public const string NullSignaturePrivateKey = NullSignaturePublicKey;
      

        public static BigInteger[] GetNullSignature()
        {
            BigInteger[] sig = new BigInteger[2];
            sig[0] = new BigInteger(NullSignaturePublicKey, 16);
            sig[1] = new BigInteger(NullSignaturePrivateKey, 16);

            return sig;
        }


        

     

    }
}
