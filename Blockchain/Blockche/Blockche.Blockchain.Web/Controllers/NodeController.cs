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
            return Ok(new {  message = "The chain was reset to its genesis block"});
        }


        // GET api/Node/Debug/Mine
        [HttpGet]
        [Route("Debug/Mine/{minerAddress}/{difficulty}")]
        public IActionResult Reset(string minerAddress, int difficulty=3)
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

        // GET api/blocks
        [HttpGet]
        [Route("Blocks/{index}")]
        public IActionResult Blocks(int index)
        {
            //TODO: handle error if index is out of range
            var result = this.GetNodeSingleton().Chain.Blocks[index];
            return Ok(result);
        }

     

      
    }
}



//app.get('/transactions/pending', (req, res) => {
//    res.json(node.chain.getPendingTransactions());
//});

//app.get('/transactions/confirmed', (req, res) => {
//    res.json(node.chain.getConfirmedTransactions());
//});

//app.get('/transactions/:tranHash', (req, res) => {
//    let tranHash = req.params.tranHash;
//    let transaction = node.chain.getTransactionByHash(tranHash);
//    if (transaction)
//        res.json(transaction);
//    else
//        res.status(HttpStatus.NOT_FOUND).json({ errorMsg: "Invalid transaction hash"});
//});

//app.get('/balances', (req, res) => {
//    let confirmedBalances = node.chain.calcAllConfirmedBalances();
//res.json(confirmedBalances);
//});

//app.get('/address/:address/transactions', (req, res) => {
//    let address = req.params.address;
//    let tranHistory = node.chain.getTransactionHistory(address);
//res.json(tranHistory);
//});

//app.get('/address/:address/balance', (req, res) => {
//    let address = req.params.address;
//    let balance = node.chain.getAccountBalance(address);
//    if (balance.errorMsg)
//        res.status(HttpStatus.NOT_FOUND);
//res.json(balance);
//});

//app.post('/transactions/send', (req, res) => {
//    let tran = node.chain.addNewTransaction(req.body);
//    if (tran.transactionDataHash) {
//        // Added a new pending transaction --> broadcast it to all known peers
//        node.broadcastTransactionToAllPeers(tran);

//        res.status(HttpStatus.CREATED).json({
//    transactionDataHash: tran.transactionDataHash
//        });
//    }
//    else
//        res.status(HttpStatus.BAD_REQUEST).json(tran);
//});

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

//node.notifyPeersAboutNewBlock = async function()
//{
//    let notification = {
//        blocksCount: node.chain.blocks.length,
//        cumulativeDifficulty: node.chain.calcCumulativeDifficulty(),
//        nodeUrl: node.selfUrl
//    };
//    for (let nodeId in node.peers) {
//        let peerUrl = node.peers[nodeId];
//logger.debug(`Notifying peer ${peerUrl} about the new block`);
//        axios.post(peerUrl + "/peers/notify-new-block", notification)
//            .then(function(){ }).catch(function(){})
//    }
//};

//node.broadcastTransactionToAllPeers = async function(tran)
//{
//    for (let nodeId in node.peers)
//    {
//        let peerUrl = node.peers[nodeId];
//        logger.debug(`Broadcasting a transaction ${ tran.transactionsHash}
//        to peer ${ peerUrl}`);
//        axios.post(peerUrl + "/transactions/send", tran)
//            .then(function(){ }).catch (function(){ })
//    }
//    };

//    node.syncChainFromPeerInfo = async function(peerChainInfo) {
//        try
//        {
//            let thisChainDiff = node.chain.calcCumulativeDifficulty();
//            let peerChainDiff = peerChainInfo.cumulativeDifficulty;
//            if (peerChainDiff > thisChainDiff)
//            {
//                logger.debug(`Chain sync started.Peer: ${ peerChainInfo.nodeUrl}. Expected chain length = ${ peerChainInfo.blocksCount}, expected cummulative difficulty = ${ peerChainDiff}.`);
//                let blocks = (await axios.get(peerChainInfo.nodeUrl + "/blocks")).data;
//                let chainIncreased = node.chain.processLongerChain(blocks);
//                if (chainIncreased)
//                {
//                    node.notifyPeersAboutNewBlock();
//                }
//            }
//        }
//        catch (err)
//        {
//            logger.error("Error loading the chain: " + err);
//        }
//    };

//    node.syncPendingTransactionsFromPeerInfo = async function(peerChainInfo) {
//        try
//        {
//            if (peerChainInfo.pendingTransactions > 0)
//            {
//                logger.debug(
//                `Pending transactions sync started.Peer: ${ peerChainInfo.nodeUrl}`);
//                let transactions = (await axios.get(
//                    peerChainInfo.nodeUrl + "/transactions/pending")).data;
//                for (let tran of transactions)
//                {
//                    let addedTran = node.chain.addNewTransaction(tran);
//                    if (addedTran.transactionDataHash)
//                    {
//                        // Added a new pending tx --> broadcast it to all known peers
//                        node.broadcastTransactionToAllPeers(addedTran);
//                    }
//                }
//            }
//        }
//        catch (err)
//        {
//            logger.error("Error loading the pending transactions: " + err);
//        }
//    };
