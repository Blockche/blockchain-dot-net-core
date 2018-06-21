using Org.BouncyCastle.Math;
using System;
using Xunit;
 

namespace Blockche.Blockchain.Common.Tests
{
    public class ValidationUtilsTests
    {
        [Fact]
        public void Test_IsValidAddress_When_Valid()
        {
            var validAddress = "c3293572dbe6ebc60de4a20ed0e21446cae66b17";

            Assert.True(ValidationUtils.IsValidAddress(validAddress));
        }

        [Fact]
        public void Test_IsValidAddress_When_Not_Valid()
        {
            var validAddress = "x3293572dbe6ebc60de4a20ed0e21446cae66b17";

            Assert.False(ValidationUtils.IsValidAddress(validAddress));
        }

        [Fact]
        public void Test_IsValidPublicKey_When_Valid()
        {
            var validAddress = "c74a8458cd7a7e48f4b7ae6f4ae9f56c5c88c0f03e7c59cb4132b9d9d1600bba1";

            Assert.True(ValidationUtils.IsValidPublicKey(validAddress));
        }

        [Fact]
        public void Test_IsValidPublicKey_When_Not_Valid()
        {
            var validAddress = "1c74a8458cd7a7e48f4b7ae6f4ae9f56c5c88c0f03e7c59cb4132b9d9d1600bba1";

            Assert.False(ValidationUtils.IsValidPublicKey(validAddress));
        }

        [Fact]
        public void Test_IsValidTransferValue_When_Valid()
        {
           
            Assert.True(ValidationUtils.IsValidTransferValue(1));
        }

        [Fact]
        public void Test_IsValidTransferValue_When_Negative()
        {
            
            Assert.False(ValidationUtils.IsValidTransferValue(-1));
        }



        [Fact]
        public void Test_IsValidDate_When_Valid()
        {
            var date = "2018-02-10T17:53:48.972Z";
            Assert.True(ValidationUtils.IsValidDate(date));
        }

        [Fact]
        public void Test_IsValidTransferValue_When_Empty_String()
        {
            var date = "";
            Assert.False(ValidationUtils.IsValidDate(date));
        }

        [Fact]
        
        public void Test_IsValidTransferValue_When_Null()
        {
            string date = null;
            Assert.Throws<ArgumentNullException>(() => ValidationUtils.IsValidDate(date));
            
        }

        [Fact]
        public void Test_IsValidDate_When_Not_Valid_Day()
        {
            var date = "2018-02-33T17:53:48.972Z";
            Assert.False(ValidationUtils.IsValidDate(date));
        }

        [Fact]
        public void Test_IsValidDate_When_Not_Valid_Month()
        {
            var date = "2018-22-10T17:53:48.972Z";
            Assert.False(ValidationUtils.IsValidDate(date));
        }

        [Fact]
        public void Test_IsValidDate_When_Not_Valid_Year()
        {
            var date = "218-22-10T37:53:48.972Z";
            Assert.False(ValidationUtils.IsValidDate(date));
        }

        [Fact]
        public void Test_IsValidDate_When_Not_Valid_Hour()
        {
            var date = "2018-22-10T37:53:48.972Z";
            Assert.False(ValidationUtils.IsValidDate(date));
        }

        [Fact]
        public void Test_IsValidDate_When_Not_Valid_YearToSmall()
        {
            var date = "2017-22-10T17:53:48.972Z";
            Assert.False(ValidationUtils.IsValidDate(date));
        }


        [Fact]
        public void Test_IsValidDate_When_Not_Valid_YearToBig()
        {
            var date = "2101-22-10T17:53:48.972Z";
            Assert.False(ValidationUtils.IsValidDate(date));
        }



        [Fact]
        public void Test_IsValidSignatureFormat_When_Valid()
        {
            BigInteger[] sig = new BigInteger[2];
            sig[0] = new BigInteger("1aaf55dcb11060749b391d547f37b4727222dcb90e793d9bdb945c64fe4968b0", 16);
            sig[1] = new BigInteger("87250a2841f7a56910b0f7ebdd067589632ccf19d352c15f16cfdba9b7687960", 16); 
            
          

            Assert.True(ValidationUtils.IsValidSignatureFormat(sig));
        }

        //[Fact]
        //public void Test_IsValidSignatureFormat_When_Not_Valid()
        //{
        //    BigInteger[] sig = new BigInteger[2];
        //    sig[0] = new BigInteger("1");
        //    sig[1] = new BigInteger("2");



        //    Assert.False(ValidationUtils.IsValidSignatureFormat(sig));
        //}

        [Fact]
        public void Test_IsValidSignatureFormat_When_Wrong_Format()
        {
            BigInteger[] sig = new BigInteger[2];
            sig[0] = new BigInteger("121231231231212312312312123123123121231231231212312312312123123123121231231231212312312312123123123121231231231212312312312123123123");
            sig[1] = new BigInteger("121231231231212312312312123123123121231231231212312312312123123123");

            Assert.False(ValidationUtils.IsValidSignatureFormat(sig));
            //Assert.ThrowsAny<FormatException>(() => ValidationUtils.IsValidSignatureFormat(sig));

            
        }


        [Fact]
        public void Test_IsValidDifficulty_When_Valid()
        {
            

            Assert.True(ValidationUtils.IsValidDifficulty("00000f1212",5));
            //Assert.ThrowsAny<FormatException>(() => ValidationUtils.IsValidSignatureFormat(sig));


        }


        [Fact]
        public void Test_IsValidDifficulty_When_Not_Valid()
        {


            Assert.False(ValidationUtils.IsValidDifficulty("00000f1212", 6));
            //Assert.ThrowsAny<FormatException>(() => ValidationUtils.IsValidSignatureFormat(sig));


        }

    }
}
