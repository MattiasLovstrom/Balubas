using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Balubas
{
    public class Wallet : IWallet
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
        public TransactionBlock CreateTransaction(double amount, string walletId)
        {
            var inputs = new List<TransactionInput>();
            var collectedAmount = 0.0;
            foreach (var transaction in _repository.UnspentTransactions(PublicKey))
            {
                foreach (var transactionOutput in transaction.Outputs)
                {
                    if(transactionOutput.Receiver != PublicKey) continue;
                    inputs.Add(new TransactionInput{ Hash = transaction.Hash, Row = transactionOutput.Row });
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
                    Row = outputs.Count
                });
            }
            outputs.Add(new TransactionOutput
            {
                Amount = amount, 
                Receiver = walletId,
                Row = outputs.Count
            });

            var transactionBlock = new TransactionBlock
            {
                PreviousHash = _repository.First().Hash,
                Inputs = inputs.ToArray(),
                Outputs = outputs.ToArray()
            };
            //transactionBlock.Hash = _crypto.CalculateHash(transactionBlock);
            transactionBlock.Sign = _crypto.Sign(transactionBlock.GetSigningData(), PrivateKey);
            return transactionBlock;
        }

        public override string ToString()
        {
            var message = new StringBuilder("(").Append(FriendlyName).Append(") ")
                .Append(nameof(PublicKey)).Append("=").Append(PublicKey.Substring(PublicKey.Length - 6)).Append(" ")
                .Append("UnspentTransactions")
                .Append("=");
            var unspentTransactions = _repository.UnspentTransactions(PublicKey).ToArray();
            if (unspentTransactions.Any())
            {
                message.Append("[");
                message.Append(unspentTransactions.Select(t => t.ToString()).Aggregate((c, n) => c + ", " + n));
                message.Append("]");
            }

            return message.ToString();
        }
    }
}
