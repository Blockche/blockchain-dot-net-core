using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Blockche.Blockchain.Web.Controllers
{
    public class TransactionsController : BaseController
    {
        [HttpGet]
        public IActionResult Details(string id)
        {
            var node = this.GetNodeSingleton();
            var transaction = node.Chain.GetTransactionByHash(id);

            return this.View(transaction);
        }
    }
}
