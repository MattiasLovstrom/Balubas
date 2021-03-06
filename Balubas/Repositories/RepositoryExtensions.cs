using System.Collections.Generic;
using System.Linq;
using Balubas.Model;

namespace Balubas.Repositories
{
    public static class RepositoryExtensions
    {
        public static bool IsUsed(this IRepository repository, string hash, int row)
        {
            return repository.Where(block => block.Inputs != null)
                .Any(block => block.Inputs.Any(transactionInput => transactionInput.Hash == hash && transactionInput.Row == row));
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

        public static double UnspentAmount(this IRepository repository, string walletPublicKey)
        {
            var unspent = UnspentTransactions(repository, walletPublicKey);
            var amount = 0d;
            foreach (var transaction in unspent)
            {
                foreach (var myOutput in transaction.Outputs.Where(o => o.Receiver == walletPublicKey))
                {
                    amount += myOutput.Amount;
                }
            }

            return amount;
        }
    }
}
