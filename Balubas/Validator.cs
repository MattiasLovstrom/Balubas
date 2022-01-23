using System;
using System.Linq;

namespace Balubas
{
    public class Validator : IValidator
    {
        private readonly IRepository _repository;
        private readonly ICryptoHandler _cryptoHandler;

        public Validator (
            IRepository repository, 
            ICryptoHandler crypto)
        {
            _repository = repository;
            _cryptoHandler = crypto;
        }

        public void Validate(TransactionBlock block)
        {
            if (block.PreviousHash != _repository.First().Hash) throw new ApplicationException($"Wrong sequence hash, previous hash expected {_repository.First().Hash} but was {block.PreviousHash}.");
            if (block.Hash != _cryptoHandler.CalculateHash(block)) throw new ApplicationException("Wrong hash.");
            var totalAmountIn = ValidateInputs(block);
            var totalAmountOut = ValidateOutputs(block);
            if (totalAmountOut != totalAmountIn) throw new ApplicationException("Input amount and output amount don't match.");
        }

        public double ValidateInputs(TransactionBlock block)
        {
            if (block.Inputs == null || !block.Inputs.Any()) throw new ApplicationException("A block need to have inputs.");
            var totalAmountIn = 0d;
            foreach (var input in block.Inputs)
            {
                var inputBlock = _repository.Get(input.Hash);
                if (!_cryptoHandler.Verify(block.GetHashData(), block.Sign, inputBlock.Outputs[input.Row].Receiver))
                {
                    throw new ApplicationException("Can't verify input transactions.");
                }

                totalAmountIn += inputBlock.Outputs[input.Row].Amount;
            }

            return totalAmountIn;
        }

        public double ValidateOutputs(TransactionBlock block)
        {
            if (!block.Outputs.Any()) throw new ApplicationException("Transactions needs to have outputs.");
            var totalAmountOut = 0d;
            foreach (var output in block.Outputs)
            {
                totalAmountOut += output.Amount;
            }
            var genesisBlock = _repository.First();
            if (!_cryptoHandler.Verify(genesisBlock.GetHashData(), genesisBlock.Sign, Genesis.PublicKey))
            {
                throw new ApplicationException("First block has to be the genesis block.");
            }

            return totalAmountOut;
        }

    }
}