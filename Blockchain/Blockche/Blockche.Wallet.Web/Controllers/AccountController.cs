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
        
        [HttpPost]
        public IActionResult GenerateAccount(string seed)
        {
            var account = CryptoUtils.GenerateNewAccount(seed);

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
            this.HttpContext.Session.SetString("PrivateKey", account.PrivateKey);

            return this.Json(account);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            this.HttpContext.Session.Remove("Address");
            this.HttpContext.Session.Remove("PrivateKey");

            return this.RedirectToAction("Index", "Home");
        }
    }
}