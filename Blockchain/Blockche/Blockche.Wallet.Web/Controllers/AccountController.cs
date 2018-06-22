using System.IO;
using System.Text;
using Blockche.Blockchain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;

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
        public IActionResult GenerateKeystore()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GenerateKeystore(string password, string seed)
        {
            var account = CryptoUtils.GenerateKeystore(password, seed);

            var json = JsonConvert.SerializeObject(account);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = "keystore.json",
                    Inline = false,
                };

                this.Response.Headers.Add("Content-Disposition", cd.ToString());

                return this.File(stream.ToArray(), "application/json");
            }
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

        [HttpGet]
        public IActionResult ImportKeystore()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ImportKeystore(IFormFile keystoreFile, string password)
        {
            var stream = keystoreFile.OpenReadStream();
            var sr = new StreamReader(stream);

            var text = sr.ReadToEnd();
            var keystore = JsonConvert.DeserializeObject<KeyStore>(text);
            

            var privateKey = CryptoUtils.GetPrivateKeyFromKeyStore(keystore, password);
            var account = CryptoUtils.GetAccountInfoForPrivateKey(privateKey);
            this.HttpContext.Session.SetString("Address", account.Address);
            this.HttpContext.Session.SetString("PrivateKey", account.PrivateKey);

            return this.RedirectToAction("Index", "Home");
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