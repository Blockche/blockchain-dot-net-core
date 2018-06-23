using System.Net.Http;
using System.Threading.Tasks;
using Blockche.Blockchain.Common;
using Blockche.Blockchain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Blockche.Wallet.Web.Controllers
{
    public class BalancesController : Controller
    {
        public async Task<IActionResult> Balance()
        {
            var address = this.HttpContext.Session.GetString("Address");
            var nodeUrl = this.HttpContext.Session.GetString("NodeUrl");

            if (string.IsNullOrEmpty(address))
            {
                return this.Unauthorized();
            }

            using (var httpClient = new HttpClient())
            {
                var request = await httpClient.GetAsync($"{nodeUrl}/api/node/balance/address/{address}");
                if (request.IsSuccessStatusCode)
                {
                    var stringResult = await request.Content.ReadAsStringAsync();
                    var model = JsonConvert.DeserializeObject<AccountBalance>(stringResult);

                    return this.View(model);
                }
            }

            return this.RedirectToAction("ImportKeystore", "Account");
        }
    }
}
