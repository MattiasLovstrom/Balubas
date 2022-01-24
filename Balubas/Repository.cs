﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Balubas
{
    public class Repository : IRepository
    {
        private readonly IValidator _validator;
        private TransactionBlock _last;

        private readonly Dictionary<string, TransactionBlock> _repository = new Dictionary<string, TransactionBlock>
        {
            {
                Genesis.Hash, Genesis.Block
            }
        };

        public Repository(
            ICryptoHandler cryptoHandler)
        {
            _validator = new Validator(this, cryptoHandler);
            _last = _repository[Genesis.Hash];
        }

        public Repository(
            ICryptoHandler cryptoHandler, 
            IValidator validator)
        {
            var cryptoHandler1 = cryptoHandler;
            _validator = validator;
            _last = _repository[Genesis.Hash];
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
