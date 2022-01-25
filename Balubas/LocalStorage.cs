using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Balubas
{
    public class LocalStorage : IRepository
    {
        private const string FileName = "balubas.db";
        private readonly ICryptoHandler _crypto;
        private readonly Validator _validator;

        public LocalStorage(
            ICryptoHandler crypto)
        {
            _crypto = crypto;
            _validator = new Validator(this, _crypto);
            if (!File.Exists(FileName)) 
            { 
                File.Create(FileName).Close();
                File.AppendAllLines(FileName, new[] { JsonSerializer.Serialize(Genesis.Block) });
            }
        }

        public IEnumerable<TransactionBlock> TransactionsTo(string walletId)
        {
            throw new System.NotImplementedException();
        }

        public bool IsUsed(string hash)
        {
            throw new System.NotImplementedException();
        }

        public TransactionBlock Get(string hash = null)
        {
            foreach (var line in File.ReadLines(FileName))
            {
                var block = JsonSerializer.Deserialize<TransactionBlock>(line);
                if (hash == null || hash == block.Hash)
                {
                    return block;
                }
            }

            return null;
        }

        public void Add(TransactionBlock transaction)
        {
            _validator.Validate(transaction);
            File.AppendAllLines(FileName, new[] { JsonSerializer.Serialize(transaction) });
        }

        public IEnumerator<TransactionBlock> GetEnumerator()
        {
            return File.ReadLines(FileName)
                .Select(line => JsonSerializer.Deserialize<TransactionBlock>(line))
                .Reverse()
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}