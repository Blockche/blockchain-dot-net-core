using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Blockche.Blockchain.Common;

namespace Blockche.Blockchain.Models
{
    public class Block
    {
        public Block(int index,
                    Transaction[] transactions,
                    int difficulty,
                    byte[] prevBlockHash,
                    string minedBy,
                    byte[] blockDataHash=null,
                    int nonce = 0, 
                    string dateCreated = null,
                    byte[] blockHash = null)
        {
            Index = index;
            Transactions = transactions;
            Difficulty = difficulty;
            PrevBlockHash = prevBlockHash;
            MinedBy = minedBy;
            BlockDataHash = blockDataHash;

            // Calculate the block data hash if it is missing
            if (BlockDataHash == null)
            {
                this.BlockDataHash = CalculateBlockDataHash();
            }

            Nonce = nonce;
            DateCreated = dateCreated;
            BlockHash = blockHash;

            // Calculate the block hash if it is missing
            if (this.BlockHash == null)
            {
                this.BlockHash = this.CalculateBlockHash();
            }
        }

        public byte[] CalculateBlockHash()
        {
            var data =string.Format("{0}|{1}|{2}", this.BlockDataHash,  this.DateCreated, this.Nonce);
            return CryptoUtils.CalcSHA256(data);
        }

        public byte[] CalculateBlockDataHash()
        {
            var blockData = new
            {
                index = this.Index,
                transactions = this.Transactions.Select(t => new
                {
                    from = t.From,
                    to = t.To,
                    value = t.Value,
                    fee = t.Fee,
                    dateCreated = t.DateCreated,
                    data = t.Data,
                    senderPubKey = t.SenderPubKey,
                    transactionDataHash = t.TransactionDataHash,
                    senderSignature = t.SenderSignature,
                    minedInBlockIndex = t.MinedInBlockIndex,
                    transferSuccessful = t.TransferSuccessful,
                }),
                difficulty = this.Difficulty,
                prevBlockHash = this.PrevBlockHash,
                minedBy = this.MinedBy
            };

            var tranDataJSON = JsonConvert.SerializeObject(blockData);
            return CryptoUtils.CalcSHA256(tranDataJSON);
        }

        public int Index { get; set; } // integer
        public Transaction[] Transactions { get; set; } // Transaction[]
        public int Difficulty { get; set; } // integer
        public byte[] PrevBlockHash { get; set; } // hex_number[64] || byte[32]
        public string MinedBy { get; set; } // address (40 hex digits)
        public byte[] BlockDataHash { get; set; } // address (40 hex digits)
        public int Nonce { get; set; }  // integer
        public string DateCreated { get; set; } // ISO8601_string
        public byte[] BlockHash { get; set; } // hex_number[64] || byte[]
    }
}
