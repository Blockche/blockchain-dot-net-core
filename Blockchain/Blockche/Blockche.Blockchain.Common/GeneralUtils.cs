using System;
using System.Collections.Generic;
using System.Text;

namespace Blockche.Blockchain.Common
{
   public class GeneralUtils
    {
        public static string NowInISO8601()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        public static string NowMinusSecondsInISO8601(int seconds)
        {
            return DateTime.UtcNow.AddSeconds(-1*seconds).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
    }
}
