using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockche.Blockchain.Web.Models
{
    public class AppData
    {
        public AppData()
        {
            this.FaucetRequests = new Dictionary<string, DateTime>();
            this.FaucetDaylyRequestsCount = new Dictionary<string, int>();
        }

        public Dictionary<string,DateTime> FaucetRequests { get; set; }

        //the key is address->DateTime -f ToShortDateString()
        public Dictionary<string, int> FaucetDaylyRequestsCount { get; set; }
    }
}
