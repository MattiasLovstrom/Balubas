using System.Collections.Generic;
using System.Linq;

namespace Balubas
{
    public static class RepositoryExtensions
    {
        public static bool IsUsed(this IRepository repository, string hash) =>
            repository
                .Where(block => block.Inputs != null)
                .Any(block => block.Inputs.Any(transactionInput => transactionInput.Hash == hash));

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

    }
}
