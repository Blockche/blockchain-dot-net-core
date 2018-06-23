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

        public static readonly Dictionary<string, string> MinerAddressBlockIndexDateTimeMap =
            new Dictionary<string, string>();

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
            var transactions = this.GetConfirmedTransactions().ToList();
            transactions.AddRange(this.PendingTransactions);
            return transactions;
        }

        public Transaction GetTransactionByHash(string tranHash)
        {
            var allTransactions = this.GetAllTransactions();
            var tran = allTransactions.FirstOrDefault(t => t.TransactionHashHex == tranHash);
            return tran;
        }

        public Block GetBlockByIndex(int index)
        {
            var blocks = this.Blocks;
            var block = blocks.FirstOrDefault(b => b.Index == index);
            return block;
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

            if (!ValidationUtils.IsValidPublicKey(tranData.SenderPubKey))
                throw new ArgumentException("Invalid public key:" + tranData.SenderPubKey);

            var senderAddr = CryptoUtils.GetAddressFromPublicKey(tranData.SenderPubKey);
            if (senderAddr != tranData.From)
                throw new ArgumentException("The public key should match the sender address:" + tranData.SenderPubKey);

            if (!ValidationUtils.IsValidTransferValue(tranData.Value))
                throw new ArgumentException("Invalid transfer value: " + tranData.Value);

            if (!ValidationUtils.IsValidFee(tranData.Fee))
                throw new ArgumentException("Invalid transaction fee: " + tranData.Fee);

            if (!ValidationUtils.IsValidDate(tranData.DateCreated))
                throw new ArgumentException("Invalid date: " + tranData.DateCreated);

            if (!ValidationUtils.IsValidSignatureFormat(CryptoUtils.SignatureByHex(tranData.SenderSignature)))
                throw new ArgumentException("Invalid or missing signature. Expected signature format: BigInteger[2] " + tranData.SenderSignature);


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

            //tran.IsSignatureValid = tranData.IsSignatureValid;

            // Check for duplicated transactions (to avoid "replay attack")
            var tranDataHex = CryptoUtils.BytesToHex(tran.TransactionDataHash);
            if (this.FindTransactionByDataHash(tranDataHex) != null)
                throw new ArgumentException("Duplicated transaction: " + tranDataHex);


            if (!CryptoUtils.VerifySignature(CryptoUtils.SignatureByHex(tran.SenderSignature), tran.TransactionDataHash))
                throw new ArgumentException("Invalid signature: " + tranData.SenderSignature);


            var balances = this.GetAccountBalance(tran.From);
            if (balances.ConfirmedBalance < tran.Fee)
                throw new ArgumentException("Unsufficient sender balance at address:  " + tran.From);

            //if (this.PendingTransactions.Any(s => CryptoUtils.BytesToHex(s.TransactionDataHash) == CryptoUtils.BytesToHex(tran.TransactionDataHash)))
            //    throw new ArgumentException("Trying to add duplicate transaction:  " + CryptoUtils.BytesToHex(tran.TransactionDataHash));

            this.PendingTransactions.Add(tran);
            Console.WriteLine("Added pending transaction: " + JsonConvert.SerializeObject(tran));

            return tran;
        }


        // @return map(address -> balance)
        public Dictionary<string, long> CalcAllConfirmedBalances()
        {
            var transactions = this.GetConfirmedTransactions().ToList();
            Dictionary<string, long> balances = new Dictionary<string, long>();
            foreach (var tran in transactions)
            {
                if (!balances.ContainsKey(tran.From))
                {
                    balances[tran.From] = 0;
                }

                if (!balances.ContainsKey(tran.To))
                {
                    balances[tran.To] = 0;
                }

                balances[tran.From] -= tran.Fee;

                if (tran.TransferSuccessful)
                {
                    balances[tran.From] -= tran.Value;
                    balances[tran.To] += tran.Value;
                }
            }
            return balances;
        }

        public Block GetMiningJob(string minerAddress)
        {
            var nextBlockIndex = this.Blocks.Count;

            // Deep clone all pending transactions & sort them by fee
            var clonedTransactions = (SerializeUtils.CloneList<Transaction>(this.PendingTransactions))
            .OrderByDescending(s => s.Fee).ToList(); // sort descending by fee

            var dateCreated = GeneralUtils.NowInISO8601();
            var key = nextBlockIndex + "_" + minerAddress;
            if (MinerAddressBlockIndexDateTimeMap.ContainsKey(key))
            {
                dateCreated = MinerAddressBlockIndexDateTimeMap[key];
            }
            else
            {
                MinerAddressBlockIndexDateTimeMap[key] = dateCreated;
            }

            // Prepare the coinbase transaction -> it will collect all tx fees
            var coinbaseTransaction = new Transaction(
                Config.NullAddress,       // from (address)
                minerAddress,             // to (address)
                Config.BlockReward,       // value (of transfer)
                0,                        // fee (for mining)
                dateCreated, // dateCreated
                "coinbase tx",            // data (payload / comments)
                Config.NullPubKey,        // senderPubKey
                null,                // transactionDataHash
                Config.GetNullSignatureHex(),     // senderSignature
                nextBlockIndex,           // minedInBlockIndex
                true
            );

            // Execute all pending transactions (after paying their fees)
            // Transfer the requested values if the balance is sufficient
            var balances = this.CalcAllConfirmedBalances();
            foreach (var tran in clonedTransactions)
            {
                if (!balances.ContainsKey(tran.From))
                {
                    balances[tran.From] = 0;
                }

                if (!balances.ContainsKey(tran.To))
                {
                    balances[tran.To] = 0;
                }

                if (balances[tran.From] >= tran.Fee)
                {
                    tran.MinedInBlockIndex = nextBlockIndex;

                    // The transaction sender pays the processing fee
                    balances[tran.From] -= tran.Fee;
                    coinbaseTransaction.Value += tran.Fee;

                    // Transfer the requested value: sender -> recipient
                    if (balances[tran.From] >= tran.Value)
                    {
                        balances[tran.From] -= tran.Value;
                        balances[tran.To] += tran.Value;
                        tran.TransferSuccessful = true;
                    }
                    else
                    {
                        tran.TransferSuccessful = false;
                    }
                }
                else
                {
                    // The transaction cannot be mined due to insufficient
                    // balance to pay the transaction fee -> drop it
                    this.RemovePendingTransactions(new List<Transaction>() { tran });
                    clonedTransactions.Remove(tran);
                }
            }

            // Insert the coinbase transaction, holding the block reward + tx fees
            coinbaseTransaction.CalculateDataHash();
            clonedTransactions.Insert(0, coinbaseTransaction);

            // Prepare the next block candidate (block template)
            var previousBlock = this.Blocks[this.Blocks.Count - 1];

            //TODO: test that with Date null??
            var nextBlockCandidate = new Block(
                nextBlockIndex,
                clonedTransactions.ToList(),
                this.CurrentDifficulty,
                previousBlock.BlockHash,
                previousBlock.Index,
                minerAddress
            );

            this.MiningJobs[CryptoUtils.BytesToHex(nextBlockCandidate.BlockDataHash)] = nextBlockCandidate;
            return nextBlockCandidate;
        }


        public void RemovePendingTransactions(IEnumerable<Transaction> transactionsToRemove)
        {

            var tranHashesToRemove = new HashSet<string>();

            foreach (var item in transactionsToRemove)
            {
                tranHashesToRemove.Add(CryptoUtils.BytesToHex(item.TransactionDataHash));
            }

            foreach (var dataHash in tranHashesToRemove)
            {
                this.PendingTransactions.RemoveAll(s => CryptoUtils.BytesToHex(s.TransactionDataHash) == dataHash);
            }


        }

        public Block MineNextBlock(string minerAddress, int difficulty)
        {
            // Prepare the next block for mining
            var oldDifficulty = this.CurrentDifficulty;
            this.CurrentDifficulty = difficulty;
            var nextBlock = this.GetMiningJob(minerAddress);
            this.CurrentDifficulty = oldDifficulty;

            // Mine the next block
            nextBlock.DateCreated = GeneralUtils.NowInISO8601();
            nextBlock.Nonce = 0;
            do
            {
                nextBlock.Nonce++;
                nextBlock.BlockHash = nextBlock.CalculateBlockHash();
            } while (!ValidationUtils.IsValidDifficulty(CryptoUtils.BytesToHex(nextBlock.BlockHash), difficulty));

            // Submit the mined block
            var newBlock = this.SubmitMinedBlock(nextBlock.BlockDataHash, // ?? thing if I have to recalculate that
                nextBlock.DateCreated, nextBlock.Nonce, nextBlock.BlockHash);
            return newBlock;
        }

        public Block SubmitMinedBlock(byte[] blockDataHash, string dateCreated, ulong nonce, byte[] blockHash)
        {
            // Find the block candidate by its data hash
            var newBlock = this.MiningJobs[CryptoUtils.BytesToHex(blockDataHash)];
            if (newBlock == null)
                throw new ArgumentException("Block not found or already mined");


            // Build the new block
            newBlock.DateCreated = dateCreated;
            newBlock.Nonce = nonce;

            newBlock.BlockHash = newBlock.CalculateBlockHash();

            // We can not compare block hash because the one we have in newBlock is not ok
            // Validate the block hash + the proof of work
            //if (CryptoUtils.BytesToHex(newBlock.BlockHash) != CryptoUtils.BytesToHex(blockHash))
            //    throw new ArgumentException("Block hash is incorrectly calculated");

            if (!ValidationUtils.IsValidDifficulty(
                    CryptoUtils.BytesToHex(newBlock.BlockHash), newBlock.Difficulty))
                throw new ArgumentException("The calculated block hash does not match the block difficulty");


            newBlock = this.ExtendChain(newBlock);

            //if (!newBlock.errorMsg)
            //    logger.debug("Mined a new block: " + JSON.stringify(newBlock));

            return newBlock;
        }

        public Block ExtendChain(Block newBlock)
        {
            if (newBlock.Index != this.Blocks.Count)
                throw new ArgumentException("The submitted block was already mined by someone else");


            var prevBlock = this.Blocks[this.Blocks.Count - 1];
            if (CryptoUtils.BytesToHex(prevBlock.BlockHash) != CryptoUtils.BytesToHex(newBlock.PrevBlockHash))
                throw new ArgumentException("Incorrect prevBlockHash");


            // The block is correct --> accept it
            this.Blocks.Add(newBlock);
            this.MiningJobs = new Dictionary<string, Block>(); // Invalidate all mining jobs
            this.RemovePendingTransactions(newBlock.Transactions);

            this.AdjustDifficulty();

            return newBlock;
        }

        private void AdjustDifficulty()
        {
            var lastBlocks = this.Blocks.TakeLast(3);
            var lastDates = lastBlocks.Select(b => GeneralUtils.FromISO8601(b.DateCreated)).ToList();
            var diffsInSeconds = new List<int>();
            for (int i = 0; i < lastDates.Count - 1; i++)
            {
                diffsInSeconds.Add(Math.Abs((lastDates[i] - lastDates[i + 1]).Seconds));
            }

            var currentDiff = diffsInSeconds.Average();
            if (Config.MinTargetSecondsBetweenBlocks <= currentDiff && currentDiff <= Config.MaxTargetSecondsBetweenBlocks)
            {
                // No need for adjusting
                return;
            }

            if (currentDiff < Config.MinTargetSecondsBetweenBlocks)
            {
                // Increaze diff
                this.CurrentDifficulty++;
            }
            else if (Config.MaxTargetSecondsBetweenBlocks < currentDiff)
            {
                this.CurrentDifficulty--;
            }
        }

        public bool ProcessLongerChain(List<Block> blocks)
        {
            // TODO: validate the chain (it should be longer, should hold valid blocks, each block should hold valid transactions, etc.
            this.Blocks = blocks;
            this.MiningJobs = new Dictionary<string, Block>(); // Invalidate all mining jobs
            this.RemovePendingTransactions(this.GetConfirmedTransactions());

            Console.WriteLine("Chain sync successful. Block count = " + blocks.Count);

            return true;
        }
    }
}
