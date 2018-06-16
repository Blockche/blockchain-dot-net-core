using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blockche.Wallet.Web.Pages
{
    public class GenerateAccountModel : PageModel
    {
        public string Text { get; set; }

        public void OnGet()
        {

        }
    }
}