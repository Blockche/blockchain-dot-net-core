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
        public Block(
            int index,
            List<Transaction> transactions,
            int difficulty,
            byte[] prevBlockHash,
            int? previousBlockIndex,
            string minedBy,
            byte[] blockDataHash = null,
            ulong nonce = 0,
            string dateCreated = null,
            byte[] blockHash = null)
        {
            this.Index = index;
            this.Transactions = transactions;
            this.Difficulty = difficulty;
            this.PrevBlockHash = prevBlockHash;
            this.MinedBy = minedBy;
            this.BlockDataHash = blockDataHash;

            // Calculate the block data hash if it is missing
            if (BlockDataHash == null)
            {
                this.BlockDataHash = CalculateBlockDataHash();
            }

            this.Nonce = nonce;
            this.DateCreated = dateCreated;
            this.BlockHash = blockHash;
            this.PrevBlockIndex = previousBlockIndex;

            // Calculate the block hash if it is missing
            if (this.BlockHash == null)
            {
                this.BlockHash = this.CalculateBlockHash();
            }
        }

        public byte[] CalculateBlockHash()
        {
            var data = string.Format("{0}|{1}|{2}", CryptoUtils.BytesToHex(this.BlockDataHash), this.DateCreated, this.Nonce);
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
        public List<Transaction> Transactions { get; set; } // Transaction[]
        public int Difficulty { get; set; } // integer
        public byte[] PrevBlockHash { get; set; } // hex_number[64] || byte[32]
        public string PrevBlockHashHex => PrevBlockHash != null ? CryptoUtils.BytesToHex(this.PrevBlockHash) : null;
        public int? PrevBlockIndex { get; set; }
        public string MinedBy { get; set; } // address (40 hex digits)
        public byte[] BlockDataHash { get; set; } // address (40 hex digits)
        public ulong Nonce { get; set; }  // integer
        public string DateCreated { get; set; } // ISO8601_string
        public byte[] BlockHash { get; set; } // hex_number[64] || byte[]
        public string BlockHashHex => CryptoUtils.BytesToHex(this.BlockHash);
    }
}
