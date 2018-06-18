using System;
using System.Linq;
using Blockche.Blockchain.Common;
using Blockche.Wallet.Web.Models;
using Microsoft.AspNetCore.Mvc;

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
            if (this.ModelState.IsValid)
            {
                var isoDate = DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
                var transactionSignature = CryptoUtils.SignTransaction(
                    model.RecipientAddress,
                    model.Value,
                    model.Fee,
                    isoDate,
                    "9e9e78143f9b04a9ea4d22e4d9f534fa974c840ffd6585831caf16a9d01acb2f"); // TODO Get From session

                ////var pubKey =
                ////    CryptoUtils.GetPublicKeyHashFromPrivateKey(
                ////        "9e9e78143f9b04a9ea4d22e4d9f534fa974c840ffd6585831caf16a9d01acb2f");

                ////var tran = new
                ////{
                ////    From = "638bfd9174ba154420c8a0d1e7c2a69bed9e36f5",
                ////    To = "638bfd9174ba154420c8a0d1e7c2a69bed9e36f5",
                ////    PublicKey = pubKey,
                ////    Value = 100,
                ////    Fee = 1,
                ////    CreatedOn = isoDate,

                ////};
                ////var tranJson = JsonConvert.SerializeObject(tran);

                ////var tranHash = CryptoUtils.CalcSHA256(tranJson);
                ////CryptoUtils.VerifySignature(
                ////    "9e9e78143f9b04a9ea4d22e4d9f534fa974c840ffd6585831caf16a9d01acb2f",
                ////    transactionSignature,
                ////    tranHash);

                return this.Json(transactionSignature.Select(x => x.ToString()));
            }

            return this.BadRequest();
        }
    }
}
