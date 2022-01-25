using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Balubas
{
    public class Wallet
    {
        private readonly IRepository _repository;
        private readonly ICryptoHandler _crypto;

        public Wallet()
        { }

        public Wallet(
            IRepository repository,
            ICryptoHandler crypto)
        {
            _repository = repository;
            _crypto = crypto;
            var keys = _crypto.CreatePrivatePublicKeys();
            PrivateKey = keys[0];
            PublicKey = keys[1];
        }

        [JsonIgnore]
        public string FriendlyName { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        [JsonIgnore]
        public IEnumerable<TransactionBlock> UnspentTransactions =>
            _repository.TransactionsTo(PublicKey)
                .Where(transaction => !_repository.IsUsed(transaction.Hash));

        public TransactionBlock CreateTransaction(double amount, string walletId)
        {
            var inputs = new List<TransactionInput>();
            var collectedAmount = 0.0;
            foreach (var transaction in UnspentTransactions)
            {
                foreach (var transactionOutput in transaction.Outputs)
                {
                    inputs.Add(new TransactionInput{ Hash = transaction.Hash});
                    collectedAmount += transactionOutput.Amount;
                    if (collectedAmount >= amount) break;
                }
            }

            var outputs = new List<TransactionOutput>();

            if (collectedAmount < amount) throw new ApplicationException("Can't find enough amount to spend.");
            if (collectedAmount > amount)
            {
                outputs.Add(new TransactionOutput
                {
                    Amount = collectedAmount - amount,
                    Receiver = PublicKey,
                });
            }
            outputs.Add(new TransactionOutput
            {
                Amount = amount, 
                Receiver = walletId
            });

            var transactionBlock = new TransactionBlock
            {
                PreviousHash = _repository.First().Hash,
                Inputs = inputs.ToArray(),
                Outputs = outputs.ToArray()
            };
            transactionBlock.Hash = _crypto.CalculateHash(transactionBlock);
            transactionBlock.Sign = _crypto.Sign(transactionBlock.GetHashData(), PrivateKey);
            return transactionBlock;
        }

        public override string ToString()
        {
            var message = new StringBuilder("Wallet (").Append(FriendlyName).Append(") ")
                .Append(PublicKey.Substring(PublicKey.Length - 6)).Append(" ")
                .Append(nameof(UnspentTransactions))
                .Append("=");
            var unspentTransactions = UnspentTransactions;
            if (unspentTransactions.Any())
            {
                message.Append(UnspentTransactions.Select(t => t.ToString()).Aggregate((c, n) => c + ", " + n));
            }

            return message.ToString();
        }

        //public bool Verify(TransactionBlock transaction)
        //{
        //    foreach (var inputHash in transaction.Inputs)
        //    {
        //        // inputs.reviver = walletId
        //        var input = _blockChain.Get(inputHash);
        //        if (input.Receiver != PublicKey) return false;
        //        // sign must be verified against inputs.Receiver
        //        if (!_crypto.Verify(input.Receiver, transaction.GetHashData(), transaction.Sign)) return false;
        //    }

        //    return true;
        //}
    }
}
