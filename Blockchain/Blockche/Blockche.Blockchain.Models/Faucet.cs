using Blockche.Blockchain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockche.Blockchain.Models
{
    public class Faucet
    {
        public const string FaucetPrivateKey = "838ff8634c41ba62467cc874ca156830ba55efe3e41ceeeeae5f3e77238f4eef";
        public static string FaucetPublicKey = CryptoUtils.GetPublicKeyHashFromPrivateKey(FaucetPrivateKey);
        public static string FaucetAddress = CryptoUtils.PublicKeyToAddress(FaucetPublicKey);

        public const string GenesisDate = "2018-01-01T00:00:00.000Z";

        public static Transaction GetGenesisFaucetTransaction()
        {
            var pubKey = CryptoUtils.HexToBytes(Config.NullPubKey);
            var signature = Config.GetNullSignature();

            var tran = new Transaction(
            Config.NullAddress,   // from address
            FaucetAddress, // to Address
            1000000000000, // value of transfer
            0,             // fee for mining
            GenesisDate,   // dateCreated
            "genesis tx",  // data (payload)
            pubKey,    // senderPubKey
            null,     // transactionDataHash
            signature, // senderSignature
            0,             // minedInBlockIndex
            true);           // transferSuccessful

            return tran;
        }


        public static Block GetGenesisBlock()
        {

            var block = new Block(
             0,           // block index
             new List<Transaction> { GetGenesisFaucetTransaction() }, // transactions array
             0,           // currentDifficulty
             null,   // previous block hash
             Config.NullAddress, // mined by (address)
             null,   // block data hash
             0,           // nonce
             GenesisDate, // date created
             null);    // block hash

            return block;

        }


    }
}
