using System;
using System.Collections.Generic;
using System.Text;

namespace Blockche.Blockchain.Models
{
    public class AccountBalance
    {
        public AccountBalance(int safeBalance = 0, int confirmedBalance = 0, int pendingBalance = 0)
        {
            SafeBalance = safeBalance;
            ConfirmedBalance = confirmedBalance;
            PendingBalance = pendingBalance;
        }

        public int SafeBalance { get; set; }
        public int ConfirmedBalance { get; set; }
        public int PendingBalance { get; set; }

    }
}
