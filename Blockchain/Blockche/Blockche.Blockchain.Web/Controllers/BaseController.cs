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
  
    public abstract class BaseController : Controller
    {
        //protected string GetSelfUrl()
        //{

        //    var host = Request.Host.Host;
        //    //TODO: Think about it
        //    int port = Request.Host.Port ?? (Request.Scheme.ToLowerInvariant().Contains("https") ? 443 : 80);
        //    var selfUrl = string.Format("http://{0}:{1}", host, port);
        //    return selfUrl;
        //}

        protected Node GetNodeSingleton()
        {
            var url = Request.Host; // will get www.mywebsite.com
            var host = url.Host;
            //TODO: Think about it
            int port = url.Port ?? (Request.Scheme.ToLowerInvariant().Contains("https") ? 443 : 80);  // if it doesn't has port, check if it's https (443) or not (80)

            var node = Node.GetInstance(host, port.ToString(),
                new Blockche.Blockchain.Models.Blockchain(Faucet.GetGenesisBlock(), Config.StartDifficulty));
            return node;
        }
    }
}