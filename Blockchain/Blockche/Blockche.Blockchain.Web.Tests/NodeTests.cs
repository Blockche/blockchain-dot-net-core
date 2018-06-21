using Blockche.Blockchain.Common;
using Blockche.Blockchain.Models;
using Blockche.Blockchain.Tests.Seed;
using System;
using System.Collections.Generic;
using Xunit;

namespace Blockche.Blockchain.Web.Tests
{
    public class NodeTests
    {


        [Fact]
        public void TestGenerateAndSignTransactions()
        {
            var trans = Seeder.GenerateAndSignTransactions();
            foreach (var t in trans)
            {
                Assert.True(t.VerifySignature());
            }
           
           
        }

        [Fact]
        public void TestAddAndMineTransactions()
        {
            List<Transaction> transactions = Seeder.GenerateAndSignTransactions();

            //this is singleton, so it will be reused from the tests
            var node = Node.GetInstance("localhost", "80",
                new Models.Blockchain(Faucet.GetGenesisBlock(), Config.StartDifficulty));

            var chain = node.Chain;
            chain.AddNewTransaction(transactions[0]); //Faucet to Alice
            chain.AddNewTransaction(transactions[1]);//Faucet to Bob


            chain.MineNextBlock(Seeder.MinerAddress, 1);

            chain.AddNewTransaction(transactions[2]); //Alice to Bob - OK
            chain.AddNewTransaction(transactions[3]);//Alice to Bob Peter - No funds

            chain.MineNextBlock(Seeder.MinerAddress, 2);

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


    }
}
