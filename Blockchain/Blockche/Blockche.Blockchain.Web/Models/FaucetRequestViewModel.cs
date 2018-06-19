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
        [MinLength(40,ErrorMessage ="The address should be exactly 40 symbols")]
        [MaxLength(40, ErrorMessage = "The address should be exactly 40 symbols")]
        public string Address { get; set; }

        
        [Display(Name = "Message")]
        [MaxLength(128, ErrorMessage = "The message should be no more than 128 symbols")]
        public string Message { get; set; }
    }
}
