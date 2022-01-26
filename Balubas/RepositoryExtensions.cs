using System.Collections.Generic;
using System.Linq;

namespace Balubas
{
    public static class RepositoryExtensions
    {
        public static bool IsUsed(this IRepository repository, string hash, int row)
        {
            foreach (var block in repository)
            {
                if (block.Inputs != null)
                {
                    if (block.Inputs.Any(transactionInput => transactionInput.Hash == hash && transactionInput.Row == row)) return true;
                }
            }

            return false;
        }


        public static IEnumerable<TransactionBlock> TransactionsTo(this IRepository repository, string walletId)
        {
            foreach (var b in repository)
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

        public static IEnumerable<TransactionBlock> UnspentTransactions(this IRepository repository, string walletPublicKey)
        {
            foreach (TransactionBlock transaction in repository.TransactionsTo(walletPublicKey))
            {
                foreach (var output in transaction.Outputs)
                {
                    if (output.Receiver != walletPublicKey) continue;
                    if (!repository.IsUsed(transaction.Hash, output.Row)) yield return transaction;
                }
            }
        }
    }
}
