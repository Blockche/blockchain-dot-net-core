using Blockche.Blockchain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockche.Blockchain.Web.Models
{
    public class AddressViewModel
    {
        public AddressViewModel()
        {
            this.TransFrom = new List<Transaction>();
            this.TransTo = new List<Transaction>();
        }
        public AccountBalance Balance { get; set; }
        public List<Transaction> TransFrom { get; set; }
        public List<Transaction> TransTo { get; set; }

    }
}
