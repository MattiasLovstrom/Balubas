using System.Collections.Generic;

namespace Balubas
{
    public interface IRepository : IEnumerable<TransactionBlock>
    {
        // Get(null) return last
        // get(hash) returns transaction if exists otherwise null
        TransactionBlock Get(string hash = null);
        void Add(TransactionBlock transaction);
    }
}