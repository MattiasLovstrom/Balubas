using System.Collections.Generic;

namespace Balubas
{
    public interface IRepository : IEnumerable<TransactionBlock>
    {
        IEnumerable<TransactionBlock> TransactionsTo(string walletId);
        bool IsUsed(string hash);
        TransactionBlock Get(string hash = null);
        void Add(TransactionBlock transaction);
    }
}