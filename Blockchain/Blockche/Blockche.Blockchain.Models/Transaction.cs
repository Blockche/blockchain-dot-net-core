﻿using Blockche.Blockchain.Common;
using Newtonsoft.Json;
using Org.BouncyCastle.Math;
using System;

namespace Blockche.Blockchain.Models
{
    [Serializable]
    public class Transaction
    {
        public Transaction(
            string from,
            string to,
            long value,
            int fee,
            string dateCreated,
            string data,
            string senderPubKey,
            byte[] transactionDataHash = null,
            string[] senderSignature = null,
            int? minedInBlockIndex = null,
            bool transferSuccessful = false)
        {
            this.From = from;
            this.To = to;
            this.Value = value;
            this.Fee = fee;
            this.DateCreated = dateCreated;
            this.Data = data;
            this.SenderPubKey = senderPubKey;
            this.TransactionDataHash = transactionDataHash;
            if (this.TransactionDataHash == null)
            {
                this.CalculateDataHash();
            }

            this.SenderSignature = senderSignature;

            this.MinedInBlockIndex = minedInBlockIndex;
            this.TransferSuccessful = transferSuccessful;
        }

        public void CalculateDataHash()
        {
            var tranData = new
            {
                from = this.From,
                to = this.To,
                value = this.Value,
                fee = this.Fee,
                dateCreated = this.DateCreated, // test if that is null
                data = this.Data,
                senderPubKey = this.SenderPubKey
            };

            var tranDataJson = JsonConvert.SerializeObject(tranData);
            this.TransactionDataHash = CryptoUtils.CalcSHA256(tranDataJson);
        }

        public string From { get; set; } // Sender address: 40 hex digits
        public string To { get; set; } // Recipient address: 40 hex digits
        public long Value { get; set; } // Transfer value: integer (int or BigInteger) ??
        public int Fee { get; set; } // Mining fee
        public string DateCreated { get; set; } // ISO-8601 string
        public string Data { get; set; } // Optional data (e.g. payload or comments): string
        public string SenderPubKey { get; set; } // 65 hex digits
        public byte[] TransactionDataHash { get; set; }
        public string TransactionHashHex => CryptoUtils.BytesToHex(this.TransactionDataHash ?? new byte[0]); // 64 hex digits
        public string[] SenderSignature { get; set; } // hex_number[2][64]
        public int? MinedInBlockIndex { get; set; } 
        public bool TransferSuccessful { get; set; }

        public bool IsSignatureValid { get; private set; } // bool

        public void SetSignature(string senderPrivKeyHex)
        {
            // convert pk to big integer
            var privateKey = new BigInteger(senderPrivKeyHex, 16);

            var sign = CryptoUtils.SignData(privateKey, this.TransactionDataHash);
            this.SenderSignature = CryptoUtils.SignatureByBigInt(sign);

            this.IsSignatureValid = CryptoUtils
                .VerifySignature(senderPrivKeyHex, sign, this.TransactionDataHash);
        }

        /// <summary>
        /// To be sure that it's right, first you have call SetSignature
        /// </summary>
        /// <returns>If the Signature is correct</returns>
        public bool VerifySignature()
        {
            return this.IsSignatureValid;
        }
    }
}
