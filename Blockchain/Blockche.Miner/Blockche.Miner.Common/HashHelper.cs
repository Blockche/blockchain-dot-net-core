using System;
using System.Security.Cryptography;
using System.Text;

namespace Blockche.Miner.Common
{
    public class HashHelper
    {
        private static readonly Lazy<SHA256Managed> sHA256 = new Lazy<SHA256Managed>(true);

        public static string ComputeSHA256(string input)
            => BitConverter.ToString(sHA256.Value.ComputeHash(Encoding.UTF8.GetBytes(input ?? string.Empty))).Replace("-", string.Empty).ToLowerInvariant();
    }
}
