using System.Collections.Generic;

namespace Balubas
{
    public interface IRepository : IEnumerable<TransactionBlock>
    {
        TransactionBlock Get(string hash = null);
        void Add(TransactionBlock transaction);
    }
}