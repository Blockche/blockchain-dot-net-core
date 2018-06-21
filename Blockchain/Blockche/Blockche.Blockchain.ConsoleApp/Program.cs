using Blockche.Blockchain.Common;
using Blockche.Blockchain.Models;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using System;

namespace Blockche.Blockchain.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            CryptoUtils.SignAndVerifyTransaction(recipientAddress: "f51362b7351ef62253a227a77751ad9b2302f911",
                 value: 25000,
                 fee: 10,
                 iso8601datetime: "2018-02-10T17:53:48.972Z",
                 senderPrivKeyHex: "7e4670ae70c98d24f3662c172dc510a085578b9ccc717e6c2f4e547edd960a34");

            string[] SampleUrls =
                    new string[] {
                        "http://localhost:5001",
                        "http://localhost:5002",
                        "http://localhost:5003",
                        "http://localhost:5004"
                    };

            NodeInstanceRunner.StartMultipleInstances(SampleUrls);

            //var test = DecompressPublicKey(Faucet.FaucetPublicKey);

        }

        public static ECPublicKeyParameters DecompressPublicKey(string pubKeyCompressed)
        {
            var pubKeyX = pubKeyCompressed.Substring(0, 64);
            var pubKeyYOdd = pubKeyCompressed.Substring(64);
            X9ECParameters curve = SecNamedCurves.GetByName("secp256k1");
            ECDomainParameters Domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

            //doesn't work
             var p = curve.Curve.DecodePoint(CryptoUtils.GetBytes(pubKeyCompressed));

            //var bi = new BigInteger(pubKeyCompressed, 16);
            // ECFieldElement p = curve.Curve.FromBigInteger(bi);

            //var p = curve.Curve.ValidatePoint(new BigInteger(pubKeyCompressed, 16), new BigInteger(pubKeyYOdd));

            //var a = Asn1OctetString.FromByteArray(CryptoUtils.HexToBytes(pubKeyX));
            //var b = a.ToAsn1Object();

            
            

             var pubKeyPoint = new ECPublicKeyParameters(p, Domain);
            return pubKeyPoint;
        }
    }
}
