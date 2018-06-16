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


        // GET api/Node/Debug/Mine
        [HttpGet]
        [Route("Debug/Mine/{minerAddress}/{difficulty}")]
        public IActionResult Reset(string minerAddress, int difficulty = 3)
        {

            var result = this.GetNodeSingleton().Chain.MineNextBlock(minerAddress, difficulty);
            return Ok(result);
        }


        // GET api/blocks
        [HttpGet]
        [Route("Blocks")]
        public IActionResult String()
        {

            var result = this.GetNodeSingleton().Chain.Blocks;
            return Ok(result);
        }

        // GET api/blocks/{index}
        [HttpGet]
        [Route("Blocks/{index}")]
        public IActionResult Blocks(int index)
        {
            //TODO: handle error if index is out of range
            var result = this.GetNodeSingleton().Chain.Blocks[index];
            return Ok(result);
        }



        // GET api/transactions/pending
        [HttpGet]
        [Route("transactions/pending")]
        public IActionResult PendingTransactions()
        {

            var result = this.GetNodeSingleton().Chain.PendingTransactions;
            return Ok(result);
        }

        // GET api/transactions/confirmed
        [HttpGet]
        [Route("transactions/confirmed")]
        public IActionResult ConfirmedTransactions()
        {

            var result = this.GetNodeSingleton().Chain.GetConfirmedTransactions();
            return Ok(result);
        }

        // GET api/transactions/{tranHash}
        [HttpGet]
        [Route("transactions/{tranHash}")]
        public IActionResult ConfirmedTransactions(string tranHash)
        {

            var tran = this.GetNodeSingleton().Chain.GetTransactionByHash(tranHash);
            return Ok(tran);

        }

        // GET api/balances
        [HttpGet]
        [Route("balances")]
        public IActionResult Balances()
        {
            var balances = this.GetNodeSingleton().Chain.CalcAllConfirmedBalances();
            return Ok(balances);

        }

        // GET api/transactions/address/{address}
        [HttpGet]
        [Route("transactions/address/{address}")]
        public IActionResult TransactionsByAddress(string address)
        {

            var tran = this.GetNodeSingleton().Chain.GetTransactionHistory(address);
            return Ok(tran);

        }

        // GET api/balance/address/{address}
        [HttpGet]
        [Route("balance/address/{address}")]
        public IActionResult GetAccountBalance(string address)
        {

            var tran = this.GetNodeSingleton().Chain.GetAccountBalance(address);
            return Ok(tran);

        }

        // GET api/transactions/send
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

   

    }
}














//app.get('/peers', (req, res) => {
//    res.json(node.peers);
//});

//app.post('/peers/connect', (req, res) => {
//    let peerUrl = req.body.peerUrl;
//    if (peerUrl === undefined)
//        return res.status(HttpStatus.BAD_REQUEST)
//            .json({ errorMsg: "Missing 'peerUrl' in the request body"});

//    logger.debug("Trying to connect to peer: " + peerUrl);
//    axios.get(peerUrl + "/info")
//        .then(function(result) {
//    if (node.nodeId === result.data.nodeId)
//    {
//        res.status(HttpStatus.CONFLICT)
//            .json({ errorMsg: "Cannot connect to self"});
//    }
//    else if (node.peers[result.data.nodeId])
//    {
//        logger.debug("Error: already connected to peer: " + peerUrl);
//        res.status(HttpStatus.CONFLICT)
//            .json({ errorMsg: "Already connected to peer: " + peerUrl});
//    }
//    else if (node.chainId !== result.data.chainId)
//    {
//        logger.debug("Error: chain ID cannot be different");
//        res.status(HttpStatus.BAD_REQUEST)
//            .json({ errorMsg: "Nodes should have the same chain ID"});
//    }
//    else
//    {
//                // Remove all peers with the same URL + add the new peer
//                for (let nodeId in node.peers)
//            if (node.peers[nodeId] === peerUrl)
//                delete node.peers[nodeId];
//        node.peers[result.data.nodeId] = peerUrl;
//        logger.debug("Successfully connected to peer: " + peerUrl);

//        // Try to connect back the remote peer to self
//        axios.post(peerUrl + "/peers/connect", { peerUrl: node.selfUrl})
//                    .then(function(){ }).catch (function(){ });

//        // Synchronize the blockchain + pending transactions
//        node.syncChainFromPeerInfo(result.data);
//        node.syncPendingTransactionsFromPeerInfo(result.data);

//        res.json({ message: "Connected to peer: " + peerUrl});
//        }
//    })
//        .catch (function(error) {
//        logger.debug(`Error: connecting to peer: ${ peerUrl}
//        failed.`);
//        res.status(HttpStatus.BAD_REQUEST).json(
//                 { errorMsg: "Cannot connect to peer: " + peerUrl});
//    });
//    });

//    app.post('/peers/notify-new-block', (req, res) => {
//    node.syncChainFromPeerInfo(req.body);
//    res.json({ message: "Thank you for the notification." });
//});

//app.get('/mining/get-mining-job/:address', (req, res) => {
//    let address = req.params.address;
//    let blockCandidate = node.chain.getMiningJob(address);
//res.json({
//        index: blockCandidate.index,
//        transactionsIncluded: blockCandidate.transactions.length,
//        difficulty: blockCandidate.difficulty,
//        expectedReward: blockCandidate.transactions[0].value,
//        rewardAddress: blockCandidate.transactions[0].to,
//        blockDataHash: blockCandidate.blockDataHash,
//    });
//});

//app.post('/mining/submit-mined-block', (req, res) => {
//    let blockDataHash = req.body.blockDataHash;
//let dateCreated = req.body.dateCreated;
//let nonce = req.body.nonce;
//let blockHash = req.body.blockHash;
//let result = node.chain.submitMinedBlock(
//    blockDataHash, dateCreated, nonce, blockHash);
//    if (result.errorMsg)
//        res.status(HttpStatus.BAD_REQUEST).json(result);
//    else {
//        res.json({"message":
//            `Block accepted, reward paid: ${result.transactions[0].value} microcoins`
//        });
//        node.notifyPeersAboutNewBlock();
//    }
//});







