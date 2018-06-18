using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blockche.Blockchain.Common;
using Blockche.Wallet.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Blockche.Wallet.Web.Controllers
{
    public class TransactionsController : Controller
    {
        [HttpGet]
        public IActionResult Sign()
        {
            return this.View(new SignTransactionInputModel());
        }

        [HttpPost]
        public IActionResult Sign(SignTransactionInputModel model)
        {
            var privateKey = this.HttpContext.Session.GetString("PrivateKey");

            if (string.IsNullOrEmpty(privateKey))
            {
                return this.Unauthorized();
            }

            if (this.ModelState.IsValid)
            {
                var isoDate = DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
                var transactionSignature = CryptoUtils.SignTransaction( // separate this method to 2; GetTransactionHash; SignTransaction
                    model.RecipientAddress,
                    model.Value,
                    model.Fee,
                    isoDate,
                    privateKey);

                var pubKey =
                    CryptoUtils.GetPublicKeyHashFromPrivateKey(
                        privateKey);

                var tran = new
                {
                    From = "9f2ee0244b4dae69d3db4e4afffa5eaab2524706",
                    To = "9f2ee0244b4dae69d3db4e4afffa5eaab2524706",
                    PublicKey = pubKey,
                    Value = 100,
                    Fee = 1,
                    CreatedOn = isoDate,

                };
                var tranJson = JsonConvert.SerializeObject(tran);

                var tranHash = CryptoUtils.CalcSHA256(tranJson);
                CryptoUtils.VerifySignature(
                    pubKey,
                    transactionSignature,
                    tranHash);

                return this.Json(transactionSignature.Select(x => x.ToString()));
            }

            return this.BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Send(SendTransactionInputModel model)
        {
            var privateKey = this.HttpContext.Session.GetString("PrivateKey");
            var address = this.HttpContext.Session.GetString("Address");

            if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(privateKey))
            {
                return this.Unauthorized();
            }

            using (var httpClient = new HttpClient())
            {
                var publicKey = CryptoUtils.GetPublicKeyHashFromPrivateKey(privateKey);
                var transactionHash = string.Empty;
                var transactionSignature = CryptoUtils.SignTransaction( // separate this method to 2; GetTransactionHash; SignTransaction
                    model.RecipientAddress,
                    model.Value,
                    model.Fee,
                    GeneralUtils.NowInISO8601(),
                    privateKey);

                // TODO: send all needed data;
                var request = await httpClient.PostAsJsonAsync("http://localihost:5001", new { publicKey, transactionHash, transactionSignature });

                if (request.IsSuccessStatusCode)
                {
                    return this.Json(await request.Content.ReadAsStringAsync());
                }
            }

            return new EmptyResult();
        }
    }
}
