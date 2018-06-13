using System;
using Xunit;

namespace Blockche.Blockchain.Common.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test_CryptoUtils_GenerateRandomKeys()
        {
            var keyPair = CryptoUtils.GenerateRandomKeys();
            Assert.True(keyPair!=null);
            Assert.True(keyPair.Private != null);
            Assert.True(keyPair.Public != null);


        }
    }
}
