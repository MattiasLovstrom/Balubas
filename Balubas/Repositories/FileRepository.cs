using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Balubas.Model;

namespace Balubas.Repositories
{
    public class FileRepository : IRepository
    {
        private const string FileName = "balubas.db";
        private readonly Validator _validator;

        public FileRepository(
            ICryptoHandler crypto)
        {
            _validator = new Validator(this, crypto);
            if (!File.Exists(FileName)) 
            { 
                File.Create(FileName).Close();
                File.AppendAllLines(FileName, new[] { JsonSerializer.Serialize(Genesis.Block) });
            }
        }

        public TransactionBlock Get(string hash = null)
        {
            var readLines = File.ReadLines(FileName);
            if (!readLines.Any()) return Genesis.Block;
            
            foreach (var line in readLines)
            {
                var block = JsonSerializer.Deserialize<TransactionBlock>(line);
                if (string.IsNullOrEmpty(hash) || hash == block.Hash)
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