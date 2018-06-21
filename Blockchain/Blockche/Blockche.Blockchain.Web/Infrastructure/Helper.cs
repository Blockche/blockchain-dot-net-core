using Blockche.Blockchain.Common;
using Blockche.Blockchain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blockche.Blockchain.Web.Infrastructure
{
    public class Helper
    {
        public static string GetSelfUrl(HttpRequest req)
        {

            var host = req.Host.Host;
            //TODO: Think about it
            int port = req.Host.Port ?? (req.Scheme.ToLowerInvariant().Contains("https") ? 443 : 80);
            var selfUrl = string.Format("http://{0}:{1}", host, port);
            return selfUrl;
        }

        public static Node GetNodeSingleton(HttpRequest req)
        {

            var url = req.Host; // will get www.mywebsite.com
            var host = url.Host;
            //TODO: Think about it
            int port = url.Port ?? (req.Scheme.ToLowerInvariant().Contains("https") ? 443 : 80);  // if it doesn't has port, check if it's https (443) or not (80)

            var node = Node.GetInstance(host, port.ToString(),
                new Blockche.Blockchain.Models.Blockchain(Faucet.GetGenesisBlock(), Config.StartDifficulty));
            return node;
        }
    }
}
