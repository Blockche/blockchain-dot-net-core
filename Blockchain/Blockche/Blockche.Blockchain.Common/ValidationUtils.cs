using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Blockche.Blockchain.Common
{
    public class ValidationUtils
    {

        public static bool IsValidAddress(string address)
        {
            var regEx = "^[0-9a-f]{40}$";
            return Regex.IsMatch(address, regEx);
        }

        public static bool IsValidPublicKey(string pubKey)
        {
            var regEx = "^[0-9a-f]{65}$";
            return Regex.IsMatch(pubKey, regEx);
        }

        public static bool IsValidTransferValue(long val)
        {

            return (val >= 0) && (val <= Config.MaxTransferValue);
        }

        public static bool IsValidFee(int fee)
        {

            return (fee >= Config.MinTransactionFee) && (fee <= Config.MaxTransactionFee);
        }






        public static bool IsValidDate(string dateISO)
        {
            var isoDateRegEx = "^[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}\\.[0-9]{2,6}Z$";
            var isMatch = Regex.IsMatch(dateISO, isoDateRegEx);
            if (!isMatch)
                return false;

            DateTime date;
            var isDate = DateTime.TryParse(dateISO, out date);
            if (!isDate)
                return false;

            var year = date.Year;
            return (year >= 2018) && (year <= 2100);
        }

        public static bool IsValidSignatureFormat(BigInteger[] signature)
        {

            if (signature.Length != 2)
                return false;
            var regEx = "^[0-9a-f]{1,65}$";
            var validNum0 = Regex.IsMatch(signature[0].ToString(16),regEx);
            var validNum1 = Regex.IsMatch(signature[1].ToString(16), regEx);
            return validNum0 && validNum1;
        }

        public static bool IsValidDifficulty(string blockHash,int difficulty)
        {
            for (var i = 0; i < difficulty; i++)
                if (blockHash[i] != '0')
                    return false;

            return true;
        }
    }
}
