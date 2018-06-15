using Blockche.Blockchain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blockche.Blockchain.Models
{
    public class Node
    {

        private static Node _instance = null;

        private static Object _mutex = new Object();

        private Node(string serverHost, string serverPort, Blockchain blockchain)
        {
            this.NodeId = Guid.NewGuid().ToString();
            this.Host = serverHost;
            this.Port = serverPort;
            this.SelfUrl = string.Format("http://{0}:{1}", serverHost, serverPort);
            this.Peers = new Dictionary<string, string>();
            this.Chain = blockchain;
            this.ChainId = CryptoUtils.BytesToHex(blockchain.Blocks[0].BlockHash);
        }

        public static Node GetInstance(string serverHost, string serverPort, Blockchain blockchain)
        {
            if (_instance == null)
            {
                lock (_mutex) // now I can claim some form of thread safety...
                {
                    if (_instance == null)
                    {
                        _instance = new Node(serverHost, serverPort, blockchain);
                    }
                }
            }

            return _instance;
        }


        public string NodeId { get; set; }  // the nodeId uniquely identifies the current node
        public string Host { get; set; }    // the external host / IP address to connect to this node
        public string Port { get; set; }    // listening TCP port number
        public string SelfUrl { get; set; } // the external base URL of the REST endpoints
        public Dictionary<string, string> Peers { get; set; }// a map(nodeId --> url) of the peers, connected to this node
        public Blockchain Chain { get; set; } // the blockchain (blocks, transactions, ...)
        public string ChainId { get; set; }  // the unique chain ID (hash of the genesis block)
    }
}
