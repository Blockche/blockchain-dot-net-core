using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blockche.Blockchain.Web.Models
{
    public class FaucetRequestViewModel
    {
        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        
        [Display(Name = "Message")]
        public string Message { get; set; }
    }
}
