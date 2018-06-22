using Microsoft.AspNetCore.Mvc;

namespace Blockche.Blockchain.Web.Controllers
{
    public class BlocksController : BaseController
    {
        public IActionResult Details(int id)
        {
            var node = this.GetNodeSingleton();
            var block = node.Chain.GetBlockByIndex(id);
            return this.View(block);
        }
    }
}
