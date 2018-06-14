using Blockche.Blockchain.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blockche.Blockchain.Models
{
    public class Blockchain
    {
        public Blockchain(Block genesisBlock,
                        int currentDifficulty)
        {
            Blocks = new List<Block>() { genesisBlock };
            PendingTransactions = new List<Transaction>();
            CurrentDifficulty = currentDifficulty;
            MiningJobs = new Dictionary<string, Block>();
        }

        public List<Block> Blocks { get; set; }// Block[]
        public List<Transaction> PendingTransactions { get; set; } // Transaction[]
        public int CurrentDifficulty { get; set; } // integer
        public Dictionary<string, Block> MiningJobs { get; set; } // map(blockDataHash => Block)


        public int CalcCumulativeDifficulty()
        {
            double difficulty = 0;
            foreach (var block in this.Blocks)
            {
                difficulty += Math.Pow(16, block.Difficulty);
            }

            return (int)difficulty;
        }

        public IEnumerable<Transaction> GetConfirmedTransactions()
        {
            var transactions = new List<Transaction>();
            foreach (var block in this.Blocks)
            {
                transactions.AddRange(block.Transactions);
            }

            return transactions;
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            List<Transaction> transactions = this.GetConfirmedTransactions().ToList();
            transactions.AddRange(this.PendingTransactions);
            return transactions;
        }

        public Transaction GetTransactionByHash(string tranHash)
        {
            var allTransactions = this.GetAllTransactions();
            var tran = allTransactions.FirstOrDefault(t => CryptoUtils.BytesToHex(t.TransactionDataHash) == tranHash);
            return tran;
        }

        public IEnumerable<Transaction> GetTransactionHistory(string address)
        {
            if (!ValidationUtils.IsValidAddress(address))
            {
                throw new ArgumentException("Invalid transaction address");
            }

            var transactionsByAddress = this.GetAllTransactions()
                .Where(
                t => t.From == address || t.To == address)
                .OrderBy(s => DateTime.Parse(s.DateCreated))
                .ToList();

            return transactionsByAddress;
        }

        public AccountBalance GetAccountBalance(string address)
        {
            if (!ValidationUtils.IsValidAddress(address))
            {
                throw new ArgumentException("Invalid account address");
            }

            var transactions = this.GetTransactionHistory(address);
            var balance = new AccountBalance();

            foreach (var tran in transactions)
            {
                var confirmsCount = 0;

                if (tran.MinedInBlockIndex.HasValue)
                {
                    confirmsCount = this.Blocks.Count - tran.MinedInBlockIndex.Value + 1;
                }



                if (tran.From == address)
                {
                    // Funds spent -> subtract value and fee
                    balance.PendingBalance -= tran.Fee;
                    if (confirmsCount == 0 || tran.TransferSuccessful)
                        balance.PendingBalance -= tran.Value;

                    if (confirmsCount >= 1)
                    {
                        balance.ConfirmedBalance -= tran.Fee;
                        if (tran.TransferSuccessful)
                            balance.ConfirmedBalance -= tran.Value;
                    }

                    if (confirmsCount >= Config.SafeConfirmCount)
                    {
                        balance.SafeBalance -= tran.Fee;
                        if (tran.TransferSuccessful)
                            balance.SafeBalance -= tran.Value;
                    }
                }

                if (tran.To == address)
                {
                    // Funds received --> add value and fee
                    if (confirmsCount == 0 || tran.TransferSuccessful)
                        balance.PendingBalance += tran.Value;
                    if (confirmsCount >= 1 && tran.TransferSuccessful)
                        balance.ConfirmedBalance += tran.Value;
                    if (confirmsCount >= Config.SafeConfirmCount && tran.TransferSuccessful)
                        balance.SafeBalance += tran.Value;
                }
            }

            return balance;
        }


        public Transaction FindTransactionByDataHash(string transactionDataHash)
        {
            var allTransactions = this.GetAllTransactions();
            var matchingTransaction = allTransactions.FirstOrDefault(
                t => CryptoUtils.BytesToHex(t.TransactionDataHash) == transactionDataHash);

            return matchingTransaction;
        }

        public Transaction AddNewTransaction(Transaction tranData)
        {
            // Validate the transaction & add it to the pending transactions
            if (!ValidationUtils.IsValidAddress(tranData.From))
                throw new ArgumentException("Invalid sender address:" + tranData.From);

            if (!ValidationUtils.IsValidAddress(tranData.To))
                throw new ArgumentException("Invalid recipient address:" + tranData.To);

            if (!ValidationUtils.IsValidPublicKey(CryptoUtils.BytesToHex(tranData.SenderPubKey)))
                throw new ArgumentException("Invalid public key:" + tranData.SenderPubKey);

            var senderAddr = CryptoUtils.PublicKeyToAddress(tranData.SenderPubKey);
            if (senderAddr != tranData.From)
                throw new ArgumentException("The public key should match the sender address:" + tranData.SenderPubKey);

            if (!ValidationUtils.IsValidTransferValue(tranData.Value))
                throw new ArgumentException("Invalid transfer value: " + tranData.Value);

            if (!ValidationUtils.IsValidFee(tranData.Fee))
                throw new ArgumentException("Invalid transaction fee: " + tranData.Fee);

            if (!ValidationUtils.IsValidDate(tranData.DateCreated))
                throw new ArgumentException("Invalid date: " + tranData.DateCreated);

            if (!ValidationUtils.IsValidSignatureFormat(tranData.SenderSignature))
                throw new ArgumentException("Invalid or missing signature. Expected signature format: BigInteger[2] " + tranData.DateCreated);


            var tran = new Transaction(
                tranData.From,
                tranData.To,
                tranData.Value,
                tranData.Fee,
                tranData.DateCreated,
                tranData.Data,
                tranData.SenderPubKey,
                null, // the transactionDataHash is auto-calculated
                tranData.SenderSignature
            );

            // Check for duplicated transactions (to avoid "replay attack")
            var tranDataHex = CryptoUtils.BytesToHex(tran.TransactionDataHash);
            if (this.FindTransactionByDataHash(tranDataHex) != null)
                throw new ArgumentException("Duplicated transaction: " + tranDataHex);


            if (!tran.VerifySignature())
                throw new ArgumentException("Invalid signature: " + tranData.SenderSignature);
            

            var balances = this.GetAccountBalance(tran.From);
            if (balances.ConfirmedBalance < tran.Value + tran.Fee)
                throw new ArgumentException("Unsufficient sender balance at address:  " + tran.From);
            

            this.PendingTransactions.Add(tran);
            Console.WriteLine("Added pending transaction: " + JsonConvert.SerializeObject(tran));

            return tran;
        }
    }
}
