using System.Collections.Generic;

namespace Balubas
{
    public interface IRepository : IEnumerable<TransactionBlock>
    {
        void Add(TransactionBlock block);
        IEnumerable<TransactionBlock> TransactionsTo(string walletId);
        bool IsUsed(string hash);
        TransactionBlock Get(string inputHash);
    }
}