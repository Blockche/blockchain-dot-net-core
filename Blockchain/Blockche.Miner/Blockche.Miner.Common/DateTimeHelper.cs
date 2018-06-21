using System;
using System.Collections.Generic;
using System.Text;

namespace Blockche.Miner.Common
{
    public static class DateTimeHelper
    {
        public static string ToISO8601(DateTime date)
            => date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        public static string UtcNowToISO8601()
            => ToISO8601(DateTime.UtcNow);
    }
}
