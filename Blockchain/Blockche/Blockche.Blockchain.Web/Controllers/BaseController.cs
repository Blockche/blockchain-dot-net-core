using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockche.Blockchain.Common;
using Blockche.Blockchain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blockche.Blockchain.Web.Controllers
{
  
    public abstract class BaseController : ControllerBase
    {
        protected Node GetNodeSingleton()
        {
            var url = Request.Host; // will get www.mywebsite.com
            var host = url.Host;
            //TODO: Think about it
            int port = url.Port ?? (Request.Scheme.ToLowerInvariant().Contains("https") ? 443 : 80);  // if it doesn't has port, check if it's https (443) or not (80)

            var node = Node.GetInstance(host, port.ToString(),
                new Models.Blockchain(Faucet.GetGenesisBlock(), Config.StartDifficulty));
            return node;
        }
    }
}