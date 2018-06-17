using Blockche.Blockchain.Common;
using System;

namespace Blockche.Blockchain.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //CryptoUtils.SignAndVerifyTransaction(recipientAddress: "f51362b7351ef62253a227a77751ad9b2302f911",
            //     value: 25000,
            //     fee: 10,
            //     iso8601datetime: "2018-02-10T17:53:48.972Z",
            //     senderPrivKeyHex: "7e4670ae70c98d24f3662c172dc510a085578b9ccc717e6c2f4e547edd960a34");

               string[] SampleUrls =
            new string[] { "http://localhost:5001", "http://localhost:5002", "http://localhost:5003", "http://localhost:5004" };

        NodeInstanceRunner.StartMultipleInstances(SampleUrls);

        }
    }
}
