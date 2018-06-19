using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blockche.Blockchain.Common;
using Blockche.Blockchain.Models;
using Blockche.Blockchain.Tests.Seed;
using Blockche.Blockchain.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blockche.Blockchain.Web.Controllers
{
    public class FaucetController : BaseController
    {
        public IActionResult Index()
        {
            var newFacelModel = new FaucetRequestViewModel();
            return View(newFacelModel);
        }

        
        public IActionResult RequestNoCoins(FaucetRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            bool isValid = true;
            string msg = "You successfully requested 1000 NoCoins";
            if (HttpContext.Items.ContainsKey(model.Address))
            {
                DateTime lastRequestTime = (DateTime)HttpContext.Items[model.Address];
                if (lastRequestTime.AddMinutes(1) > DateTime.Now)
                {
                    isValid = false;
                    msg = "You should wait a bit more to request more NoCoins!";
                }

            }

            HttpContext.Items[model.Address] = DateTime.Now;

            if (isValid)
            {
                var faucetTran = new Transaction(
                  Faucet.FaucetAddress,     // from address
                  model.Address,             // to address
                  500000,                   // value of transfer
                  Config.MinTransactionFee, // fee
                  GeneralUtils.NowInISO8601(),    // dateCreated
                 model.Message ?? "Faucet ->" + model.Address,        // data (payload / comments)
                  Faucet.FaucetPublicKey    // senderPubKey
                  );

                faucetTran.SetSignature(Faucet.FaucetPrivateKey);

                //posting the Faucet tran to other controller, where the tran is added and broadcasted
                WebRequester.Post(this.GetSelfUrl() + "/api/Node/transactions/send", faucetTran);

               // this.GetNodeSingleton().Chain.AddNewTransaction(faucetTran);
                //TODO:think about if I should mine the tran or not??

                //this.GetNodeSingleton().Chain.MineNextBlock(Seeder.MinerAddress, 1);
            }
            return RedirectToAction("RequestResult", new { success = isValid, msg = msg });
        }

        public IActionResult RequestResult(bool success, string msg)
        {
            var result = new FaucetRequestResultViewModel();
            result.Message = msg;
            result.Success = success;

            return View(result);
        }
    }
}