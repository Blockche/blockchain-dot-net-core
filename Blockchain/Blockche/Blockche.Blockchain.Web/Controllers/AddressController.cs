using System.Linq;
using Blockche.Blockchain.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blockche.Blockchain.Web.Controllers
{
    public class AddressController : BaseController
    {
        [HttpGet]
        public IActionResult Details(string id)
        {
            AddressViewModel model = new AddressViewModel();
            var node = this.GetNodeSingleton();
            model.Balance = node.Chain.GetAccountBalance(id);
            model.TransFrom = node.Chain.GetAllTransactions().Where(s => s.From == id).ToList();
            model.TransTo = node.Chain.GetAllTransactions().Where(s => s.To == id).ToList();

            return this.View(model);
        }
    }
}
