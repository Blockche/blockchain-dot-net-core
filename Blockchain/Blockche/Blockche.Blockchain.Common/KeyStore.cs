using Nethereum.KeyStore.Model;

namespace Blockche.Blockchain.Common
{
    public class KeyStore
    {
        public string Id { get; set; }

        public string Ciphertext { get; set; }

        public string Cipher { get; set; }

        public string CipherParamIv { get; set; }  

        public string Kdf { get; set; }

        public KdfParameters KdfParameters { get; set; }

        public string Mac { get; set; }
    }
}
