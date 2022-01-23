using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Balubas
{
    public class Repository : IEnumerable<TransactionBlock>, IBlockChain
    {
        public const string GenesisHash = "0000000000000000000";
        public static readonly TransactionBlock GenesisBlock = new TransactionBlock
        {
            PreviousHash = null,
            Hash = GenesisHash,
            Inputs = new string[0],
            Outputs = new[]
            {
                new TransactionOutput
                {
                    Receiver = "22J9pZ1JEBrDxhrdojPUEc4aQPz7mAHmJN9vAL7xN41QTD2HYPne2Trus6j3CDQe6safsAZk9WEkXN1Xjhxh65X22cZLAP2uuMp",
                    Amount = 1000000,
                    Sign = ""
                }
            },
            Nonce = 0,
            TimeStamp = new DateTime(2022, 01, 21)
        };
        private readonly ICryptoHandler _cryptoHandler;
        private TransactionBlock _last;
        private readonly Dictionary<string, TransactionBlock> _repository = new Dictionary<string, TransactionBlock>
        {
            {
                GenesisBlock.Hash, GenesisBlock
            }
        };

        public Repository(
            ICryptoHandler cryptoHandler)
        {
            _cryptoHandler = cryptoHandler;
            _last = _repository[GenesisBlock.Hash];
        }

        public void Add(TransactionBlock block)
        {
            if (block.PreviousHash != _last.Hash) throw new ApplicationException("Wrong sequence hash.");
            if (block.Hash != _cryptoHandler.CalculateHash(block)) throw new ApplicationException("Wrong hash.");
            if (!block.Outputs.Any()) throw new ApplicationException("Transactions needs to have outputs.");
            if (!block.Inputs.Any()) throw new ApplicationException("A block need to have inputs.");
            foreach (var inputHash in block.Inputs)
            {
                var input = Get(inputHash);
                foreach (var transactionOutput in input.Outputs)
                {
                    if (!_cryptoHandler.Verify(block.GetHashData(), block.Sign, transactionOutput.Receiver))
                    {
                        throw new ApplicationException("Can't verify transaction.");
                    }
                }
            }
            //output amount must be queual to input amount
            //first block must be genisis block 

            _last = block;
            _repository.Add(block.Hash, block);
        }

        public TransactionBlock Get(string hash)
        {
            return _repository[hash];
        }

        public IEnumerable<TransactionBlock> TransactionsTo(string walletId)
        {
            foreach (var b in this)
            {
                foreach (var transactionOutput in b.Outputs)
                {
                    if (transactionOutput.Receiver == walletId)
                    {
                        yield return b;
                    }
                }
                
            }
        }

        public bool IsUsed(string hash)
        {
            foreach (var block in this)
            {
                if (block.Inputs == null) continue;
                if (block.Inputs.Contains(hash)) return true;
            }

            return false;
        }

        public IEnumerator<TransactionBlock> GetEnumerator()
        {
            var current = _last;
            while (current != null)
            {
                yield return current;
                current = current.PreviousHash != null
                    ? _repository[current.PreviousHash]
                    : null;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            var message = new StringBuilder();
            foreach (var block in this)
            {
                message.AppendLine(block.ToString());
            }

            return message.ToString();
        }
    }
}
