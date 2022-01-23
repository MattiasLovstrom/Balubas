using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Balubas
{
    public class Repository : IRepository
    {
        public const string GenesisHash = "0000000000000000000";
        public const string GenesisBlockPublicKey = "22J9pZ1JEBrDxhrdojPUEc4aQPz7mAHmJN9vAL7xN41QTD2HYPne2Trus6j3CDQe6safsAZk9WEkXN1Xjhxh65X22cZLAP2uuMp";
        public const double GenesisAmount = 1000000;

        public static readonly TransactionBlock GenesisBlock = new TransactionBlock
        {
            PreviousHash = null,
            Hash = GenesisHash,
            Inputs = new TransactionInputs
            {
                new TransactionInput{Hash = GenesisHash, Row = 0}
            },
            Outputs = new[]
            {
                new TransactionOutput
                {
                    Receiver = GenesisBlockPublicKey,
                    Amount = GenesisAmount,
                    Sign = ""
                }
            },
            Nonce = 0,
            TimeStamp = new DateTime(2022, 01, 21)
        };
        private readonly ICryptoHandler _cryptoHandler;
        private readonly IValidator _validator;
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
            _validator = new Validator(this, _cryptoHandler);
            _last = _repository[GenesisBlock.Hash];
        }

        public Repository(
            ICryptoHandler cryptoHandler, 
            IValidator validator)
        {
            _cryptoHandler = cryptoHandler;
            _validator = validator;
            _last = _repository[GenesisBlock.Hash];
        }

        public void Add(TransactionBlock block)
        {
            _validator.Validate(block);
            
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
                foreach (var transactionInput in block.Inputs)
                {
                    if (transactionInput.Hash == hash) return true;
                }
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
