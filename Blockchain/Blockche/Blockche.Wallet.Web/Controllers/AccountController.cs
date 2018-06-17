using Blockche.Blockchain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockche.Wallet.Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Generate()
        {
            return View();
        }

        //Ajax
        [HttpPost]
        public IActionResult GenerateAccount()
        {
            // save to session
            var account = CryptoUtils.GenerateNewAccount();
            this.HttpContext.Session.SetString("Address", account.Address);

            return this.Json(account);
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Import(string pk)
        {
            var account = CryptoUtils.GetAccountInfoForPrivateKey(pk);
            this.HttpContext.Session.SetString("Address", account.Address);

            return this.Json(account);
        }
    }
}