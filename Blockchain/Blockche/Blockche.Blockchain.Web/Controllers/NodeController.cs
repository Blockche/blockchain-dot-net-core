using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockche.Blockchain.Common;
using Blockche.Blockchain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

            var aboutData = new AboutInfo()
            {
                About = "BlokcheChain/0.01-C#",
                NodeId = node.NodeId,
                ChainId = node.ChainId,
                NodeUrl = node.SelfUrl,
                Peers = node.Peers.Keys.Count,
                CurrentDifficulty = node.Chain.CurrentDifficulty,
                BlocksCount = node.Chain.Blocks.Count,
                CumulativeDifficulty = node.Chain.CalcCumulativeDifficulty(),
                ConfirmedTransactions = node.Chain.GetConfirmedTransactions().Count(),
                PendingTransactions = node.Chain.PendingTransactions.Count,
            };


            return Ok(aboutData);
        }

        // GET api/Node/Debug
        [HttpGet]
        [Route("Debug")]
        public IActionResult Debug()
        {

            var data = this.GetNodeSingleton().Chain.CalcAllConfirmedBalances();
            return Ok(data);
        }


        // GET api/Node/Debug/Reset
        [HttpGet]
        [Route("Debug/Reset")]
        public IActionResult Reset()
        {

            var data = this.GetNodeSingleton().Chain = new Models.Blockchain(Faucet.GetGenesisBlock(), Config.StartDifficulty);
            return Ok(new { message = "The chain was reset to its genesis block" });
        }


        // GET api/Node/Debug/Mine/{minerAddress}/{difficulty}
        [HttpGet]
        [Route("Debug/Mine/{minerAddress}/{difficulty}")]
        public IActionResult Reset(string minerAddress, int difficulty = 3)
        {

            var result = this.GetNodeSingleton().Chain.MineNextBlock(minerAddress, difficulty);
            return Ok(result);
        }


        // GET api/Node/blocks
        [HttpGet]
        [Route("Blocks")]
        public IActionResult String()
        {

            var result = this.GetNodeSingleton().Chain.Blocks;
            return Ok(result);
        }

        // GET api/Node/blocks/{index}
        [HttpGet]
        [Route("Blocks/{index}")]
        public IActionResult Blocks(int index)
        {
            //TODO: handle error if index is out of range
            var result = this.GetNodeSingleton().Chain.Blocks[index];
            return Ok(result);
        }



        // GET api/Node/transactions/pending
        [HttpGet]
        [Route("transactions/pending")]
        public IActionResult PendingTransactions()
        {

            var result = this.GetNodeSingleton().Chain.PendingTransactions;
            return Ok(result);
        }

        // GET api/Node/transactions/confirmed
        [HttpGet]
        [Route("transactions/confirmed")]
        public IActionResult ConfirmedTransactions()
        {

            var result = this.GetNodeSingleton().Chain.GetConfirmedTransactions();
            return Ok(result);
        }

        // GET api/Node/transactions/{tranHash}
        [HttpGet]
        [Route("transactions/{tranHash}")]
        public IActionResult ConfirmedTransactions(string tranHash)
        {

            var tran = this.GetNodeSingleton().Chain.GetTransactionByHash(tranHash);
            return Ok(tran);

        }

        // GET api/Node/balances
        [HttpGet]
        [Route("balances")]
        public IActionResult Balances()
        {
            var balances = this.GetNodeSingleton().Chain.CalcAllConfirmedBalances();
            return Ok(balances);

        }

        // GET api/Node/transactions/address/{address}
        [HttpGet]
        [Route("transactions/address/{address}")]
        public IActionResult TransactionsByAddress(string address)
        {

            var tran = this.GetNodeSingleton().Chain.GetTransactionHistory(address);
            return Ok(tran);

        }

        // GET api/Node/balance/address/{address}
        [HttpGet]
        [Route("balance/address/{address}")]
        public IActionResult GetAccountBalance(string address)
        {

            var tran = this.GetNodeSingleton().Chain.GetAccountBalance(address);
            return Ok(tran);

        }

        // POST api/Node/transactions/send
        [HttpPost]
        [Route("transactions/send")]
        public IActionResult SendTransaction(Transaction tran)
        {

            if (tran.TransactionDataHash != null)
            {
                // Added a new pending transaction --> broadcast it to all known peers
                this.GetNodeSingleton().BroadcastTransactionToAllPeers(tran);

                return Ok(new { transactionDataHash = tran.TransactionDataHash });

            }

            return BadRequest("TransactionDataHash value missing:");
        }

        // GET api/Node/peers
        [HttpGet]
        [Route("peers")]
        public IActionResult Peers()
        {

            return Ok(this.GetNodeSingleton().Peers);
        }


        // POST api/Node/peers/connect
        [HttpPost]
        [Route("peers/connect")]
        public IActionResult ConnectoToPeer(Peer info)
        {
            var peerUrl = info.NodeUrl;
            if (string.IsNullOrEmpty(peerUrl))
                return BadRequest("Missing 'peerUrl' in the request body");

            Console.WriteLine("Trying to connect to peer: " + peerUrl);
            try
            {
                var node = this.GetNodeSingleton();
                var result = JsonConvert.DeserializeObject<AboutInfo>(WebRequester.Get(peerUrl + "/api/Node/about"));
                if (node.NodeId == result.NodeId)
                {
                    return BadRequest("Cannot connect to self");
                }
                else if (node.Peers.ContainsKey(result.NodeId))
                {
                    return BadRequest("Error: already connected to peer: " + peerUrl);
                }
                else if (node.ChainId != result.ChainId)
                {
                    return BadRequest("Error: chain ID cannot be different");
                }
                else
                {
                    // Remove all peers with the same URL + add the new peer ?????
                    //why - isn't this handled by the second check??
                    //foreach (var nodeId in node.Peers)
                    //    if (node.Peers[nodeId.Key] == peerUrl)
                    //        node.Peers.Remove(nodeId.Key);

                    



                    node.Peers[result.NodeId] = peerUrl;
                    Console.WriteLine("Successfully connected to peer: " + peerUrl);

                    if (!info.IsRecursive)
                    {
                        // Try to connect back the remote peer to self
                        WebRequester.Post(peerUrl + "/api/Node/peers/connect", new Peer() { NodeUrl = node.SelfUrl, IsRecursive = true });
                    }
                  


                    // Synchronize the blockchain + pending transactions
                    node.SyncChainFromPeerInfo(result);
                    node.SyncPendingTransactionsFromPeerInfo(result);

                    return Ok(new { message = "Connected to peer: " + peerUrl });
                }
            }

            catch (Exception ex)
            {

                Console.WriteLine("Error: connecting to peer: {0} failed", peerUrl);
                return BadRequest(string.Format( "Cannot connect to peer: {0}, due to {1}", peerUrl, ex.Message));

            }

        }

        // POST api/Node/peers/notify-new-block
        [HttpPost]
        [Route("/peers/notify-new-block")]
        public IActionResult NotifyNewBlock(AboutInfo info)
        {

            this.GetNodeSingleton().SyncChainFromPeerInfo(info);
            return Ok(new { message = "Thank you for the notification." });
        }

        // GET api/Node/mining/get-mining-job/{address}
        [HttpGet]
        [Route("mining/get-mining-job/{address}")]
        public IActionResult GetMiningJob(string address)
        {
            var blockCandidate = this.GetNodeSingleton().Chain.GetMiningJob(address);

         //retun candidate block
            return Ok(new {
                index= blockCandidate.Index,
                transactionsIncluded= blockCandidate.Transactions.Count,
                difficulty = blockCandidate.Difficulty,
                expectedReward = blockCandidate.Transactions[0].Value,
                rewardAddress= blockCandidate.Transactions[0].To,
                blockDataHash =CryptoUtils.BytesToHex( blockCandidate.BlockDataHash)
            });
        }


        // POST api/Node/mining/submit-mined-block
        [HttpPost]
        [Route("/mining/submit-mined-block")]
        public IActionResult SubmitMinedBlock(Block block)
        {
            //let blockDataHash = req.body.blockDataHash;
            //let dateCreated = req.body.dateCreated;
            //let nonce = req.body.nonce;
            //let blockHash = req.body.blockHash;
            try
            {
                var result = this.GetNodeSingleton().Chain.SubmitMinedBlock(
                block.BlockDataHash, block.DateCreated, block.Nonce, block.BlockHash);
                this.GetNodeSingleton().NotifyPeersAboutNewBlock();

                return Ok(new { 

                    message = string.Format("Block accepted, reward paid: {0} microcoins", result.Transactions[0].Value)

                    });

                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        



    }
}


























