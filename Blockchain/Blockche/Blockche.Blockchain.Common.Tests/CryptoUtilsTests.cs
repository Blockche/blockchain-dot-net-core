using Blockche.Blockchain.Models;
using Blockche.Blockchain.Tests.Seed;
using System;
using Xunit;


namespace Blockche.Blockchain.Common.Tests
{
    public class CryptoUtilsTests
    {
        [Fact]
        public void Test_CryptoUtils_GenerateRandomKeys()
        {
            var keyPair = CryptoUtils.GenerateRandomKeys();
            Assert.True(keyPair != null);
            Assert.True(keyPair.Private != null);
            Assert.True(keyPair.Public != null);


        }

        [Fact]
        public void Test_CryptoUtils_GenerateSha256HashInHex()
        {
            //for reference: https://jsfiddle.net/gm7boy2p/1106/
            var expResult = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08";

            var result = CryptoUtils.CalcSHA256InHex("test");
            Assert.True(expResult == result);
            Assert.True(expResult.Length == 64);

        }

        [Fact]
        public void Test_CryptoUtils_HexToBytesAndViseVersa()
        {

            var expResult = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08";
            var bytes = CryptoUtils.HexToBytes(expResult);
            Assert.True(bytes.Length == 32);
            var resultString = CryptoUtils.BytesToHex(bytes);
            Assert.True(expResult == resultString);
            Assert.True(expResult.Length == 64);

        }


        [Fact]
        public void Test_CryptoUtils_RecoverPublicKey()
        {
            var trans = Seeder.GenerateAndSignTransactions();
            foreach (var t in trans)
            {

                Assert.True(t.VerifySignature());

                var signatureBI = CryptoUtils.SignatureByHex(t.SenderSignature);

                var ecPoint = CryptoUtils.RecoverPubKeyFromSIgnatureAndHash(
                  signatureBI,
                   t.TransactionDataHash);

                var isTransactionValid = CryptoUtils.VerifySignature(ecPoint, signatureBI, t.TransactionDataHash);

                Assert.True(isTransactionValid);
            }



        }



    }
}
