using Blockche.Blockchain.Common;
using Blockche.Blockchain.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace Blockche.Blockchain.Web.Tests
{
    public class NodeTests
    {
        public static string AlicePrivateKey = "7af3f7cf01c366b608b590fa80b96bdb93b8dae18d8c3f267553086159ce30a0";
        public static string AliceAddress = CryptoUtils.PrivateKeyToAddress(AlicePrivateKey);
        public static string AlicePubKey = CryptoUtils.GetPublicKeyHashFromPrivateKey(AlicePrivateKey);

        public static string BobPrivateKey = "540e6d801169f47e11f16f07b9b92a33b0aae821feede9b0614d9b49a426cda8";
        public static string BobAddress = CryptoUtils.PrivateKeyToAddress(BobPrivateKey);

        public static string MinerPrivateKey = "dd75955d3a8e0c0bbacc01f8452a70cf6c27c436a5a8bf0d53dba97a8de9a299";
        public static string MinerAddress = CryptoUtils.PrivateKeyToAddress(MinerPrivateKey);

        public static string PeterPrivateKey = "2f445ff86ec6375b4950cfbd61eea02e65833ead44811d30ace36dd2aff74186";
        public static string PeterAddress = CryptoUtils.PrivateKeyToAddress(PeterPrivateKey);
        public static string PeterPubKey = CryptoUtils.GetPublicKeyHashFromPrivateKey(PeterPrivateKey);

        [Fact]
        public void TestGenerateAndSignTransactions()
        {
           GenerateAndSignTransactions();

           
        }

        [Fact]
        public void TestAddAndMineTransactions()
        {
            List<Transaction> transactions = GenerateAndSignTransactions();
                
            //this is singleton, so it will be reused from the tests
            var node = Node.GetInstance("localhost", "80",
                new Models.Blockchain(Faucet.GetGenesisBlock(), Config.StartDifficulty));

            var chain = node.Chain;
            chain.AddNewTransaction(transactions[0]); //Faucet to Alice
            chain.AddNewTransaction(transactions[1]);//Faucet to Bob


            chain.MineNextBlock(MinerAddress, 1);

            chain.AddNewTransaction(transactions[2]); //Alice to Bob - OK
            chain.AddNewTransaction(transactions[3]);//Alice to Bob Peter - No funds

            chain.MineNextBlock(MinerAddress, 2);

            chain.AddNewTransaction(transactions[4]);//Pending tran (not mined)

        }

        [Fact]
        public void TestChainBalances()
        {
            

            //this is singleton, so it will be reused from the tests
            var node = Node.GetInstance("localhost", "80",
                new Models.Blockchain(Faucet.GetGenesisBlock(), Config.StartDifficulty));

            var chain = node.Chain;

            var balances = chain.CalcAllConfirmedBalances();
            Assert.True(balances["f3a1e69b6176052fcc4a3248f1c5a91dea308ca9"] == 999998799980);
            Assert.True(balances["84ede81c58f5c490fc6e1a3035789eef897b5b35"] == 10000060);
            Assert.True(balances["a1de0763f26176c6d68cc77e0a1c2c42045f2314"] == 99960);
            Assert.True(balances["b3d72ad831b3e9cdbdaeda5ff4ae8e9cf182e548"] == 1100000);
           
            Assert.True(chain.PendingTransactions.Count == 1);

        }

        private static List<Transaction> GenerateAndSignTransactions()
        {
            List<Transaction> transactions = new List<Transaction>();

            var aliceFaucetTran = new Transaction(
            Faucet.FaucetAddress,     // from address
            AliceAddress,             // to address
            500000,                   // value of transfer
            Config.MinTransactionFee, // fee
            GeneralUtils.NowMinusSecondsInISO8601(900),    // dateCreated
            "Faucet -> Alice",        // data (payload / comments)
            Faucet.FaucetPublicKey    // senderPubKey
            );

            aliceFaucetTran.SetSignature(Faucet.FaucetPrivateKey);

            Assert.True(aliceFaucetTran.VerifySignature());

            var bobFaucetTran = new Transaction(
            Faucet.FaucetAddress,     // from address
            BobAddress,             // to address
            700000,                   // value of transfer
            Config.MinTransactionFee, // fee
            GeneralUtils.NowMinusSecondsInISO8601(800),    // dateCreated
            "Faucet -> Bob",        // data (payload / comments)
            Faucet.FaucetPublicKey    // senderPubKey
            );

            bobFaucetTran.SetSignature(Faucet.FaucetPrivateKey);

            Assert.True(bobFaucetTran.VerifySignature());


            var aliceToBobTranOK = new Transaction(
            AliceAddress,     // from address
            BobAddress,             // to address
            400000,                   // value of transfer
            20, // fee
            GeneralUtils.NowMinusSecondsInISO8601(700),    // dateCreated
            "Alice -> Bob",        // data (payload / comments)
            AlicePubKey    // senderPubKey
            );

            aliceToBobTranOK.SetSignature(AlicePrivateKey);

            Assert.True(aliceToBobTranOK.VerifySignature());

            var aliceToPeterTranNoFunds = new Transaction(
            AliceAddress,     // from address
            PeterAddress,             // to address
            400000,                   // value of transfer
            20, // fee
            GeneralUtils.NowMinusSecondsInISO8601(700),    // dateCreated
            "Alice -> Peter (no funds)",        // data (payload / comments)
            AlicePubKey    // senderPubKey
            );

            aliceToPeterTranNoFunds.SetSignature(AlicePrivateKey);

            Assert.True(aliceToPeterTranNoFunds.VerifySignature());


            var alicePendingFaucetTran = new Transaction(
              Faucet.FaucetAddress,     // from address
              AliceAddress,             // to address
              400000,                   // value of transfer
              Config.MinTransactionFee, // fee
              GeneralUtils.NowMinusSecondsInISO8601(400),    // dateCreated
              "Faucet -> Alice (again)",        // data (payload / comments)
              Faucet.FaucetPublicKey    // senderPubKey
              );

            alicePendingFaucetTran.SetSignature(Faucet.FaucetPrivateKey);

            Assert.True(alicePendingFaucetTran.VerifySignature());

            transactions.Add(aliceFaucetTran);
            transactions.Add(bobFaucetTran);
            transactions.Add(aliceToBobTranOK);
            transactions.Add(aliceToPeterTranNoFunds);
            transactions.Add(alicePendingFaucetTran);
            return transactions;
        }
    }
}
