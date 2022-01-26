using System;
using System.Diagnostics;

namespace Balubas
{
    public class Miner
    {
        private readonly ICryptoHandler _crypto;

        public Miner(
            ICryptoHandler crypto)
        {
            _crypto = crypto;
        }

        public int Difficulty { get; set; } = 2;

        public void Mine(TransactionBlock transaction)
        {
            var startWith = "".PadRight(Difficulty, '0');
            var random = new Random();
            transaction.Hash = "";
            do
            {
                transaction.Nonce = (ulong)random.Next(int.MaxValue);
                transaction.Hash = _crypto.CalculateHash(transaction);
                Trace.TraceInformation(transaction.Hash);
            } while (!transaction.Hash.StartsWith(startWith));
        }
    }
}