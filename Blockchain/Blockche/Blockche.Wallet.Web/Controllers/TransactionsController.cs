using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blockche.Blockchain.Common;
using Blockche.Wallet.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockche.Wallet.Web.Controllers
{
    public class TransactionsController : Controller
    {
        [HttpGet]
        public IActionResult Send()
        {
            return this.View(new SendTransactionInputModel());
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
                var creationDate = GeneralUtils.NowInISO8601();
                var publicKey = CryptoUtils.GetPublicKeyHashFromPrivateKey(privateKey);
                var transactionHash = CryptoUtils.GetTransactionHash(model.RecipientAddress, model.Value, model.Value, creationDate, address, publicKey);
                var transactionSignature = CryptoUtils
                    .SignTransaction(
                        transactionHash,
                        privateKey)
                    .Select(s => s.ToString(16));

                // TODO: send all needed data;
                var request = await httpClient.PostAsJsonAsync("http://localhost:59415/api/Node/transactions/send", new
                {
                    From = address,
                    To = model.RecipientAddress,
                    model.Value,
                    model.Fee,
                    DateCreated = creationDate,
                    SenderPubKey = publicKey,
                    SenderSignature = transactionSignature
                });

                if (request.IsSuccessStatusCode)
                {
                    return this.Json(await request.Content.ReadAsStringAsync());
                }

                return this.BadRequest();
            }
        }
    }
}
