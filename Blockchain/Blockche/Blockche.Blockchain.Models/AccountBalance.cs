using System;
using System.Collections.Generic;
using System.Text;

namespace Blockche.Blockchain.Models
{
    public class AccountBalance
    {
        public AccountBalance(long safeBalance = 0, long confirmedBalance = 0, long pendingBalance = 0)
        {
            SafeBalance = safeBalance;
            ConfirmedBalance = confirmedBalance;
            PendingBalance = pendingBalance;
        }

        public long SafeBalance { get; set; }
        public long ConfirmedBalance { get; set; }
        public long PendingBalance { get; set; }

    }
}
