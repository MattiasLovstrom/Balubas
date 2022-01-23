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
        public const string GenesisBlockPublicKey = "22J9pZ1JEBrDxhrdojPUEc4aQPz7mAHmJN9vAL7xN41QTD2HYPne2Trus6j3CDQe6safsAZk9WEkXN1Xjhxh65X22cZLAP2uuMp";
        public const double GenesisAmount = 1000000;

        public static readonly TransactionBlock GenesisBlock = new TransactionBlock
        {
            PreviousHash = null,
            Hash = GenesisHash,
            Inputs = new []
            {
                new TransactionInput{Hash = GenesisHash, Row = 0}, 
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
            var totalAmountIn = 0d;
            foreach (var input in block.Inputs)
            {
                var inputBlock = Get(input.Hash);
                if (!_cryptoHandler.Verify(block.GetHashData(), block.Sign, inputBlock.Outputs[input.Row].Receiver))
                {
                    throw new ApplicationException("Can't verify transaction.");
                }

                totalAmountIn += inputBlock.Outputs[input.Row].Amount;
            }
            var totalAmountOut = 0d;
            foreach (var output in block.Outputs)
            {
                totalAmountOut += output.Amount;
            }
            if (totalAmountOut != totalAmountIn) throw new ApplicationException("Input amount and output amount don't match.");
            var genesisBlock = this.First();
            if (!_cryptoHandler.Verify(genesisBlock.GetHashData(), genesisBlock.Sign, GenesisBlockPublicKey))
            {
                throw new ApplicationException("First block has to be the genesis block.");
            }

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
