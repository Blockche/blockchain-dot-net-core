using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Asn1.Sec;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Signers;

namespace Blockche.Blockchain.Common
{
    public class CryptoUtils
    {
        
        static readonly X9ECParameters curve = SecNamedCurves.GetByName("secp256k1");
        private static readonly ECDomainParameters Domain = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

        public static ECPoint GetPublicKeyFromPrivateKey(BigInteger privKey)
        {
            ECPoint pubKey = curve.G.Multiply(privKey).Normalize();
            return pubKey;
        }

        public static string BytesToHex(byte[] bytes)
        {
            return string.Concat(bytes.Select(s => s.ToString("x2")));
        }

        public static byte[] HexToBytes(string hexString)
        {
            int NumberChars = hexString.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
               // var len = i + 2 <= NumberChars ? 2 : 1;
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return bytes;
        }

       

        public static byte[] GetBytes(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            return bytes;
        }

        public static string CalcRIPEMD160(string text)
        {
            byte[] bytes = GetBytes(text);
            RipeMD160Digest digest = new RipeMD160Digest();
            digest.BlockUpdate(bytes, 0, bytes.Length);
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return BytesToHex(result);

        }

        public static string CalcRIPEMD160(byte[] keyBytes)
        {
            RipeMD160Digest digest = new RipeMD160Digest();
            digest.BlockUpdate(keyBytes, 0, keyBytes.Length);
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return BytesToHex(result);

        }

        public static string PublicKeyToAddress(byte[] keyBytes)
        {
            return CalcRIPEMD160(keyBytes);
        }

        public static string PublicKeyToAddress( string key)
        {
            return CalcRIPEMD160(key);
        }


        public static byte[] CalcSHA256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            Sha256Digest digest = new Sha256Digest();
            digest.BlockUpdate(bytes, 0, bytes.Length);
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return result;
        }

        public static string CalcSHA256InHex(string text)
        {
            byte[] bytes = GetBytes(text);
            Sha256Digest digest = new Sha256Digest();
            digest.BlockUpdate(bytes, 0, bytes.Length);
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return BytesToHex(result);

        }

        public static AsymmetricCipherKeyPair GenerateRandomKeys(int keySize = 256)
        {
            ECKeyPairGenerator gen = new ECKeyPairGenerator();
            SecureRandom secureRandom = new SecureRandom();
            KeyGenerationParameters keyGenParams = new KeyGenerationParameters(secureRandom, keySize);
            gen.Init(keyGenParams);
            return gen.GenerateKeyPair();
        }

        public static string EncodeECPointHexCompressed(ECPoint point)
        {
            BigInteger x = point.XCoord.ToBigInteger();
            BigInteger y = point.YCoord.ToBigInteger();

            return x.ToString(16) + Convert.ToInt32(y.TestBit(0));
        }


        private static void RandomPrivateKeyToAddress()
        {
            Console.WriteLine("Random private key --> public key --> address");
            Console.WriteLine("---------------------------------------------");

            var keyPair = GenerateRandomKeys();

            BigInteger privateKey = ((ECPrivateKeyParameters)keyPair.Private).D;
            Console.WriteLine("Private key (hex): " + privateKey.ToString(16));
            Console.WriteLine("Private key: " + privateKey.ToString(10));

            ECPoint pubKey = ((ECPublicKeyParameters)keyPair.Public).Q;
            Console.WriteLine("Public key: ({0}, {1})",
                pubKey.XCoord.ToBigInteger().ToString(10),
                pubKey.YCoord.ToBigInteger().ToString(10));

            string pubKeyCompressed = EncodeECPointHexCompressed(pubKey);
            Console.WriteLine("Public key (compressed): " + pubKeyCompressed);

            string addr = CalcRIPEMD160(pubKeyCompressed);
            Console.WriteLine("Blockchain address: " + addr);
        }

        private static void ExistingPrivateKeyToAddress(string privKeyHex)
        {
            Console.WriteLine("Existing private key --> public key --> address");
            Console.WriteLine("-----------------------------------------------");

            BigInteger privateKey = new BigInteger(privKeyHex, 16);
            Console.WriteLine("Private key (hex): " + privateKey.ToString(16));
            Console.WriteLine("Private key: " + privateKey.ToString(10));

            ECPoint pubKey = GetPublicKeyFromPrivateKey(privateKey);
            Console.WriteLine("Public key: ({0}, {1})",
                pubKey.XCoord.ToBigInteger().ToString(10),
                pubKey.YCoord.ToBigInteger().ToString(10));

            string pubKeyCompressed = EncodeECPointHexCompressed(pubKey);
            Console.WriteLine("Public key (compressed): " + pubKeyCompressed);

            string addr = CalcRIPEMD160(pubKeyCompressed);
            Console.WriteLine("Blockchain address: " + addr);
        }

        public static string  GetPublicKeyHashFromPrivateKey(string senderPrivKeyHex)
        {
            BigInteger privateKey = new BigInteger(senderPrivKeyHex, 16);

            ECPoint pubKey = GetPublicKeyFromPrivateKey(privateKey);
            string senderPubKeyCompressed = EncodeECPointHexCompressed(pubKey);

            return senderPubKeyCompressed;
        }


        public static void SignAndVerifyTransaction(string recipientAddress, int value, int fee,
           string iso8601datetime, string senderPrivKeyHex)
        {
            Console.WriteLine("Generate and sign a transaction");
            Console.WriteLine("-------------------------------");

            Console.WriteLine("Sender private key:", senderPrivKeyHex);
            BigInteger privateKey = new BigInteger(senderPrivKeyHex, 16);

            ECPoint pubKey = GetPublicKeyFromPrivateKey(privateKey);
            string senderPubKeyCompressed = EncodeECPointHexCompressed(pubKey);
            Console.WriteLine("Public key (compressed): " + senderPubKeyCompressed);

            string senderAddress = CalcRIPEMD160(senderPubKeyCompressed);
            Console.WriteLine("Blockchain address: " + senderAddress);

            var tran = new
            {
                from = senderAddress,
                to = recipientAddress,
                senderPubKey = senderPubKeyCompressed,
                value = value,
                fee = fee,
                dateCreated = iso8601datetime,
               
            };
            string tranJson = JsonConvert.SerializeObject(tran);
            Console.WriteLine("Transaction (JSON): {0}", tranJson);

            byte[] tranHash = CalcSHA256(tranJson);
            Console.WriteLine("Transaction hash(sha256): {0}", BytesToHex(tranHash));

            BigInteger[] tranSignature = SignData(privateKey, tranHash);
            Console.WriteLine("Transaction signature: [{0}, {1}]",
                tranSignature[0].ToString(16), tranSignature[1].ToString(16));

            var tranSigned = new
            {
                from = senderAddress,
                to = recipientAddress,
                senderPubKey = senderPubKeyCompressed,
                value = value,
                fee=fee,
                dateCreated = iso8601datetime,
                senderSignature = new string[]
                {
                    tranSignature[0].ToString(16),
                    tranSignature[1].ToString(16)
                }
            };

            string signedTranJson = JsonConvert.SerializeObject(tranSigned, Formatting.Indented);
            Console.WriteLine("Signed transaction (JSON):");
            Console.WriteLine(signedTranJson);

            //verify transaction
            ECPublicKeyParameters exPubKey = ToPublicKey(senderPrivKeyHex);
            bool isVerified = VerifySignature(exPubKey, tranSignature, tranHash);
            Console.WriteLine("Is the signature valid: " + isVerified);
        }

        public static ECPublicKeyParameters ToPublicKey(string privateKey)
        {
            BigInteger d = new BigInteger(privateKey, 16);
            var q = Domain.G.Multiply(d);
            var publicParams = new ECPublicKeyParameters(q, Domain);
            return publicParams;
        }

        /// <summary>
        /// Calculates deterministic ECDSA signature (with HMAC-SHA256), based on secp256k1 and RFC-6979.
        /// </summary>
        public static BigInteger[] SignData(BigInteger privateKey, byte[] data)
        {
            ECPrivateKeyParameters keyParameters = new ECPrivateKeyParameters(privateKey,Domain);
            IDsaKCalculator kCalculator = new HMacDsaKCalculator(new Sha256Digest());
            ECDsaSigner signer = new ECDsaSigner(kCalculator);

            signer.Init(true, keyParameters);
            BigInteger[] signature = signer.GenerateSignature(data);

            return signature;
        }

        public static bool VerifySignature(ECPublicKeyParameters keyParameters,BigInteger[] signature,byte[] msg )
        {
            IDsaKCalculator kCalculator = new HMacDsaKCalculator(new Sha256Digest());
            ECDsaSigner signer = new ECDsaSigner(kCalculator);
            signer.Init(false, keyParameters);

            return signer.VerifySignature(msg, signature[0], signature[1]);
        }

        public static bool VerifySignature(string privateKeyHex, BigInteger[] signature, byte[] msg)
        {
            ECPublicKeyParameters exPubKey = ToPublicKey(privateKeyHex);
            bool isVerified = VerifySignature(exPubKey, signature, msg);
            return isVerified;
        }


    }
}
