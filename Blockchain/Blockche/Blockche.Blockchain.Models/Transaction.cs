using Newtonsoft.Json;
using System;

namespace Blockche.Blockchain.Models
{

    public class Transaction
    {
        public Transaction(string from,
                           string to,
                           int value,
                           int fee,
                           string dateCreated,
                           string data,
                           string senderPubKey,
                           string transactionDataHash,
                           string senderSignature,
                           int minedInBlockIndex,
                           bool transferSuccessful)
        {
            this.From = from;
            this.To = to;
            this.Value = value;
            this.Fee = fee;
            this.DateCreated = dateCreated;
            this.Data = data;
            this.SenderPubKey = senderPubKey;
            this.TransactionDataHash = transactionDataHash;
            if (string.IsNullOrEmpty(this.TransactionDataHash))
            {
                this.CalculateDataHash();
            }

            this.SenderSignature = senderSignature;
            this.MinedInBlockIndex = minedInBlockIndex;
            this.TransferSuccessful = transferSuccessful;
        }


        private void CalculateDataHash()
        {
            var tranData = new
            {
                from = this.From,
                to = this.To,
                value = this.Value,
                fee = this.Fee,
                dateCreated = this.DateCreated,
                data = this.Data,
                senderPubKey = this.SenderPubKey
            };



            var tranDataJSON = JsonConvert.SerializeObject(tranData);
            this.TransactionDataHash = null; // CryptoUtils.sha256(tranDataJSON);
        }

        public string From { get; set; } // Sender address: 40 hex digits
        public string To { get; set; }// Recipient address: 40 hex digits
        public int Value { get; set; }// Transfer value: integer (int or BigInteger) ??
        public int Fee { get; set; }// Mining fee: integer
        public string DateCreated { get; set; } // ISO-8601 string
        public string Data { get; set; }// Optional data (e.g. payload or comments): string
        public string SenderPubKey { get; set; } // 65 hex digits
        public string TransactionDataHash { get; set; }// 64 hex digits
        public string SenderSignature { get; set; } // hex_number[2][64]
        public int MinedInBlockIndex { get; set; } // integer
        public bool TransferSuccessful { get; set; } // bool

    }
}
