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
    [Route("api/[controller]")]
    [ApiController]
    public class NodeController : BaseController
    {
        // GET api/Node/About
        [HttpGet]
        [Route("About")]
        public IActionResult About()
        {
            var node = this.GetNodeSingleton();

            var aboutData = new
            {
                about = "BlokcheChain/0.01-C#",
                nodeId = node.NodeId,
                chainId = node.ChainId,
                nodeUrl = node.SelfUrl,
                peers = node.Peers.Keys.Count,
                currentDifficulty = node.Chain.CurrentDifficulty,
                blocksCount = node.Chain.Blocks.Count,
                cumulativeDifficulty = node.Chain.CalcCumulativeDifficulty(),
                confirmedTransactions = node.Chain.GetConfirmedTransactions().Count(),
                pendingTransactions = node.Chain.PendingTransactions.Count,
            };


            return Ok(aboutData);
        }

        // GET api/Node/About
        [HttpGet]
        [Route("Debug")]
        public IActionResult Debug()
        {

            var data = this.GetNodeSingleton().Chain.CalcAllConfirmedBalances();
            return Ok(data);
        }
    }
}