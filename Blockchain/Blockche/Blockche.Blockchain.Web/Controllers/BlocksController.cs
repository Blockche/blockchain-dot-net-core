using Microsoft.AspNetCore.Mvc;

namespace Blockche.Blockchain.Web.Controllers
{
    public class BlocksController : BaseController
    {
        public IActionResult Details(int id)
        {
            var node = this.GetNodeSingleton();
            var block = node.Chain.GetBlockByIndex(id);
            if (block == null)
            {
                return this.RedirectToAction("Index", "Explorer"); // TODO: add error temp data and visualize it
            }
            return this.View(block);
        }

        public IActionResult Txs(int id)
        {
            var node = this.GetNodeSingleton();
            var block = node.Chain.GetBlockByIndex(id);

            if (block == null)
            {
                return this.RedirectToAction("Index", "Explorer"); // TODO: add error temp data and visualize it
            }

            return this.View(block.Transactions);
        }
    }
}
