using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.KeyStore.Crypto;
using Nethereum.KeyStore.Model;
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
            var pubKey = curve.G.Multiply(privKey).Normalize();

            return pubKey;
        }

        public static string BytesToHex(byte[] bytes)
        {
            return string.Concat(bytes.Select(s => s.ToString("x2")));
        }

        public static byte[] HexToBytes(string hexString)
        {
            var NumberChars = hexString.Length;
            var bytes = new byte[NumberChars / 2];
            for (var i = 0; i < NumberChars; i += 2)
            {
                // var len = i + 2 <= NumberChars ? 2 : 1;
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return bytes;
        }

        public static BigInteger[] SignatureByHex(string[] sigArr)
        {
            var sig = new BigInteger[2];
            sig[0] = new BigInteger(sigArr[0], 16);
            sig[1] = new BigInteger(sigArr[1], 16);

            return sig;
        }

        public static string[] SignatureByBigInt(BigInteger[] sigBigInt)
        {
            var sign = new string[2];
            sign[0] = sigBigInt[0].ToString(16);
            sign[1] = sigBigInt[1].ToString(16);

            return sign;
        }

        public static byte[] GetBytes(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            return bytes;
        }

        public static string CalcRIPEMD160(string text)
        {
            var bytes = GetBytes(text);
            var digest = new RipeMD160Digest();
            digest.BlockUpdate(bytes, 0, bytes.Length);
            var result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return BytesToHex(result);

        }

        public static string CalcRIPEMD160(byte[] keyBytes)
        {
            var digest = new RipeMD160Digest();
            digest.BlockUpdate(keyBytes, 0, keyBytes.Length);
            var result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return BytesToHex(result);

        }

        public static string GetAddressFromPublicKey(byte[] keyBytes)
        {
            return CalcRIPEMD160(keyBytes);
        }

        public static string GetAddressFromPublicKey(string key)
        {
            return CalcRIPEMD160(key);
        }


        public static byte[] CalcSHA256(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var digest = new Sha256Digest();
            digest.BlockUpdate(bytes, 0, bytes.Length);
            var result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return result;
        }

        public static string CalcSHA256InHex(string text)
        {
            var bytes = GetBytes(text);
            var digest = new Sha256Digest();
            digest.BlockUpdate(bytes, 0, bytes.Length);
            var result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return BytesToHex(result);
        }

        public static AsymmetricCipherKeyPair GenerateRandomKeys(string seed = null, int keySize = 256)
        {
            var gen = new ECKeyPairGenerator();
            var secureRandom = new SecureRandom();
            if (seed != null)
            {
                var seedBytes = Encoding.ASCII.GetBytes(seed);
                secureRandom.SetSeed(seedBytes);
            }
            var keyGenParams = new KeyGenerationParameters(secureRandom, keySize);
            gen.Init(keyGenParams);
            return gen.GenerateKeyPair();
        }

        public static KeyStore GenerateKeystore(string password, string seed)
        {
            var secureRandom = new SecureRandom();

            var privateKey = GenerateRandomPrivateKey(seed).ToString(16);
            var salt = secureRandom.GenerateSeed(16);

            const int cost = 8192;
            const int blockSize = 8;
            const int parallelization = 1;
            const int dkLength = 256;

            var decriptionKey = GenerateSCryptKey(
                password.HexToByteArray(),
                salt,
                cost,
                blockSize,
                parallelization,
                dkLength);

            var iv = secureRandom.GenerateSeed(16);
            var ciphartext = BytesToHex(AesCtrEncrypt(privateKey.HexToByteArray(), iv, decriptionKey));

            var mac = CalcSHA256InHex(BytesToHex(decriptionKey) + ciphartext);

            var keystore = new KeyStore
            {
                Cipher = "aes-128-ctr",
                Ciphertext = ciphartext,
                Id = Guid.NewGuid().ToString(),
                CipherParamIv = BytesToHex(iv),
                Kdf = "script",
                KdfParameters = new KdfParameters
                {
                    DkLength = dkLength,
                    N = cost,
                    Salt = BytesToHex(salt),
                    P = parallelization,
                    R = blockSize
                },
                Mac = mac
            };

            return keystore;
        }

        public static string GetPrivateKeyFromKeyStore(KeyStore keystore, string password)
        {
            if (keystore.Cipher != "aes-128-ctr")
            {
                throw new ArgumentException("Invalid Chipher");
            }

            if (keystore.Kdf != "script")
            {
                throw new ArgumentException("Invalid Kdf");
            }

            byte[] decriptionKey = GenerateSCryptKey(
                password.HexToByteArray(),
                keystore.KdfParameters.Salt.HexToByteArray(),
                keystore.KdfParameters.N,
                keystore.KdfParameters.R,
                keystore.KdfParameters.P,
                keystore.KdfParameters.DkLength);

            var mac = CalcSHA256InHex(BytesToHex(decriptionKey) + keystore.Ciphertext);

            if (mac != keystore.Mac)
            {
                throw new ArgumentException("Invalid password");
            }

            return BytesToHex(
                AesCtrDecrypt(
                    keystore.Ciphertext.HexToByteArray(),
                    keystore.CipherParamIv.HexToByteArray(),
                    decriptionKey));
        }



        public static byte[] AesCtrEncrypt(byte[] message, byte[] iv, byte[] key)
        {
            KeyStoreCrypto keyStoreCrypto = new KeyStoreCrypto();
            byte[] encryptedMessage = keyStoreCrypto.GenerateAesCtrCipher(iv, key, message);
            return encryptedMessage;
        }

        public static byte[] AesCtrDecrypt(byte[] input, byte[] iv, byte[] key)
        {
            KeyStoreCrypto keyStoreCrypto = new KeyStoreCrypto();
            byte[] decryptedMessage = keyStoreCrypto.GenerateAesCtrDeCipher(iv, key, input);
            return decryptedMessage;
        }

        public static byte[] GenerateSCryptKey(
            byte[] passphrase,
            byte[] salt,
            int cost,
            int blockSize,
            int parallelization,
            int desiredKeyBitLength)
        {
            byte[] key = Org.BouncyCastle.Crypto.Generators.SCrypt.Generate(
                passphrase,
                salt,
                cost,
                blockSize,
                parallelization,
                desiredKeyBitLength / 8);
            return key;
        }

        public static string EncodeECPointHexCompressed(ECPoint point)
        {
            var x = point.XCoord.ToBigInteger();
            var y = point.YCoord.ToBigInteger();

            return x.ToString(16) + Convert.ToInt32(y.TestBit(0));
        }

        public static BigInteger GenerateRandomPrivateKey(string seed = null)
        {
            var keyPair = GenerateRandomKeys(seed);

            return ((ECPrivateKeyParameters)keyPair.Private).D;
        }

        public static AccountInfo GenerateNewAccount(string seed)
        {
            var privateKey = GenerateRandomPrivateKey(seed);

            var privateKeyHexString = privateKey.ToString(16);

            var pubKey = GetPublicKeyHashFromPrivateKey(privateKeyHexString);
            var address = GetAddressFromPublicKey(pubKey);

            return new AccountInfo { Address = address, PrivateKey = privateKeyHexString, PublicKey = pubKey };
        }

        public static AccountInfo GetAccountInfoForPrivateKey(string privateKey)
        {
            var publicKey = GetPublicKeyHashFromPrivateKey(privateKey);
            var address = GetAddressFromPublicKey(publicKey);

            return new AccountInfo { Address = address, PrivateKey = privateKey, PublicKey = publicKey };
        }

        public static string PrivateKeyToAddress(string privKeyHex)
        {
            Console.WriteLine("Existing private key --> public key --> address");
            Console.WriteLine("-----------------------------------------------");

            var privateKey = new BigInteger(privKeyHex, 16);
            Console.WriteLine("Private key (hex): " + privateKey.ToString(16));
            Console.WriteLine("Private key: " + privateKey.ToString(10));

            var pubKey = GetPublicKeyFromPrivateKey(privateKey);
            Console.WriteLine("Public key: ({0}, {1})",
                pubKey.XCoord.ToBigInteger().ToString(10),
                pubKey.YCoord.ToBigInteger().ToString(10));

            var pubKeyCompressed = EncodeECPointHexCompressed(pubKey);
            Console.WriteLine("Public key (compressed): " + pubKeyCompressed);

            var addr = CalcRIPEMD160(pubKeyCompressed);
            Console.WriteLine("Blockchain address: " + addr);
            return addr;
        }

        private static void ExistingPrivateKeyToAddress(string privKeyHex)
        {
            PrivateKeyToAddress(privKeyHex);
        }

        public static string GetPublicKeyHashFromPrivateKey(string senderPrivKeyHex)
        {
            var privateKey = new BigInteger(senderPrivKeyHex, 16);

            var pubKey = GetPublicKeyFromPrivateKey(privateKey);
            var senderPubKeyCompressed = EncodeECPointHexCompressed(pubKey);

            return senderPubKeyCompressed;
        }

        public static byte[] GetTransactionHash(
            string recipientAddress,
            int value,
            int fee,
            string iso8601datetime,
            string senderAddress,
            string senderPublicKey)
        {
            // TODO Use custom class here
            var tran = new
            {
                From = senderAddress,
                To = recipientAddress,
                PublicKey = senderPublicKey,
                Value = value,
                Fee = fee,
                CreatedOn = iso8601datetime,

            };
            var tranJson = JsonConvert.SerializeObject(tran);

            var tranHash = CalcSHA256(tranJson);

            return tranHash;
        }

        public static BigInteger[] SignTransaction(
            byte[] transactionHash,
            string senderPrivateKeyHex)
        {
            var privateKey = new BigInteger(senderPrivateKeyHex, 16);

            var tranSignature = SignData(privateKey, transactionHash);

            return tranSignature;
        }

        public static void SignAndVerifyTransaction(string recipientAddress, int value, int fee,
           string iso8601datetime, string senderPrivKeyHex)
        {
            Console.WriteLine("Generate and sign a transaction");
            Console.WriteLine("-------------------------------");

            Console.WriteLine("Sender private key:", senderPrivKeyHex);
            var privateKey = new BigInteger(senderPrivKeyHex, 16);

            var pubKey = GetPublicKeyFromPrivateKey(privateKey);
            var senderPubKeyCompressed = EncodeECPointHexCompressed(pubKey);
            Console.WriteLine("Public key (compressed): " + senderPubKeyCompressed);

            var senderAddress = CalcRIPEMD160(senderPubKeyCompressed);
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
            var tranJson = JsonConvert.SerializeObject(tran);
            Console.WriteLine("Transaction (JSON): {0}", tranJson);

            var tranHash = CalcSHA256(tranJson);
            Console.WriteLine("Transaction hash(sha256): {0}", BytesToHex(tranHash));

            var tranSignature = SignData(privateKey, tranHash);
            Console.WriteLine("Transaction signature: [{0}, {1}]",
                tranSignature[0].ToString(16), tranSignature[1].ToString(16));

            var tranSigned = new
            {
                from = senderAddress,
                to = recipientAddress,
                senderPubKey = senderPubKeyCompressed,
                value = value,
                fee = fee,
                dateCreated = iso8601datetime,
                senderSignature = new string[]
                {
                    tranSignature[0].ToString(16),
                    tranSignature[1].ToString(16)
                }
            };

            var signedTranJson = JsonConvert.SerializeObject(tranSigned, Formatting.Indented);
            Console.WriteLine("Signed transaction (JSON):");
            Console.WriteLine(signedTranJson);

            //verify transaction
            var exPubKey = ToPublicKey(senderPrivKeyHex);
            var isVerified = VerifySignature(exPubKey, tranSignature, tranHash);
            Console.WriteLine("Is the signature valid: " + isVerified);
        }

        public static ECPublicKeyParameters ToPublicKey(string privateKey)
        {
            var d = new BigInteger(privateKey, 16);
            var q = Domain.G.Multiply(d);
            var publicParams = new ECPublicKeyParameters(q, Domain);
            return publicParams;
        }

        /// <summary>
        /// Calculates deterministic ECDSA signature (with HMAC-SHA256), based on secp256k1 and RFC-6979.
        /// </summary>
        public static BigInteger[] SignData(BigInteger privateKey, byte[] data)
        {
            var keyParameters = new ECPrivateKeyParameters(privateKey, Domain);
            IDsaKCalculator kCalculator = new HMacDsaKCalculator(new Sha256Digest());
            var signer = new ECDsaSigner(kCalculator);

            signer.Init(true, keyParameters);
            var signature = signer.GenerateSignature(data);

            return signature;
        }

        public static bool VerifySignature(ECPoint ecPoint, BigInteger[] signature, byte[] msg)
        {
            var keyParameters = new ECPublicKeyParameters(ecPoint, Domain);
            return VerifySignature(keyParameters, signature, msg);
        }

        public static bool VerifySignature(ECPublicKeyParameters keyParameters, BigInteger[] signature, byte[] msg)
        {
            IDsaKCalculator kCalculator = new HMacDsaKCalculator(new Sha256Digest());
            var signer = new ECDsaSigner(kCalculator);
            signer.Init(false, keyParameters);

            return signer.VerifySignature(msg, signature[0], signature[1]);
        }

        public static bool VerifySignature(string privateKeyHex, BigInteger[] signature, byte[] msg)
        {
            var exPubKey = ToPublicKey(privateKeyHex);
            var isVerified = VerifySignature(exPubKey, signature, msg);
            return isVerified;
        }


        public static bool VerifySignature(BigInteger[] signature, byte[] hash)
        {
            var ecPoint = CryptoUtils.RecoverPubKeyFromSIgnatureAndHash(
                 signature,
                 hash);

            var isTransactionValid = CryptoUtils.VerifySignature(ecPoint, signature, hash);
            return isTransactionValid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sig"></param>
        /// <param name="hash"></param>
        /// <param name="recid">????</param>
        /// <param name="check">???</param>
        /// <returns></returns>
        public static ECPoint ECDSA_SIG_recover_key_GFp(BigInteger[] sig, byte[] hash, int recid, bool check)
        {

            var i = recid / 2;

            Console.WriteLine("r: " + BytesToHex(sig[0].ToByteArrayUnsigned()));
            Console.WriteLine("s: " + BytesToHex(sig[1].ToByteArrayUnsigned()));

            var order = curve.N;
            var field = (curve.Curve as FpCurve).Q;
            var x = order.Multiply(new BigInteger(i.ToString())).Add(sig[0]);
            if (x.CompareTo(field) >= 0) throw new Exception("X too large");

            Console.WriteLine("Order: " + BytesToHex(order.ToByteArrayUnsigned()));
            Console.WriteLine("Field: " + BytesToHex(field.ToByteArrayUnsigned()));

            var compressedPoint = new Byte[x.ToByteArrayUnsigned().Length + 1];
            compressedPoint[0] = (byte)(0x02 + (recid % 2));
            Buffer.BlockCopy(x.ToByteArrayUnsigned(), 0, compressedPoint, 1, compressedPoint.Length - 1);
            var R = curve.Curve.DecodePoint(compressedPoint);

            Console.WriteLine("R: " + BytesToHex(R.GetEncoded()));

            if (check)
            {
                var O = R.Multiply(order);
                if (!O.IsInfinity) throw new Exception("Check failed");
            }

            var n = (curve.Curve as FpCurve).Q.ToByteArrayUnsigned().Length * 8;
            var e = new BigInteger(1, hash);
            if (8 * hash.Length > n)
            {
                e = e.ShiftRight(8 - (n & 7));
            }

            e = BigInteger.Zero.Subtract(e).Mod(order);
            var rr = sig[0].ModInverse(order);
            var sor = sig[1].Multiply(rr).Mod(order);
            var eor = e.Multiply(rr).Mod(order);
            var Q = curve.G.Multiply(eor).Add(R.Multiply(sor));

            Console.WriteLine("n: " + n);
            Console.WriteLine("e: " + BytesToHex(e.ToByteArrayUnsigned()));
            Console.WriteLine("rr: " + BytesToHex(rr.ToByteArrayUnsigned()));
            Console.WriteLine("sor: " + BytesToHex(sor.ToByteArrayUnsigned()));
            Console.WriteLine("eor: " + BytesToHex(eor.ToByteArrayUnsigned()));
            Console.WriteLine("Q: " + BytesToHex(Q.GetEncoded()));

            return Q;
        }

        public static ECPoint RecoverPubKeyFromSIgnatureAndHash(BigInteger[] sig, byte[] hash)
        {
            var recid = -1;
            for (var rec = 0; rec < 4; rec++)
            {
                try
                {
                    var Q = ECDSA_SIG_recover_key_GFp(sig, hash, rec, true);
                    return Q;
                    //if (BytesToHex(publicKey.Q.GetEncoded()).Equals(BytesToHex(Q.GetEncoded())))
                    //{
                    //    recid = rec;
                    //    break;
                    //}
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            throw new ArgumentException("Did not find proper recid");
        }




    }
}
