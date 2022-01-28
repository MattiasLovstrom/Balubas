using System.Collections.Generic;
using Balubas.Model;

namespace Balubas.Repositories
{
    public interface IRepository : IEnumerable<TransactionBlock>
    {
        // Get(null) return last
        // get(hash) returns transaction if exists otherwise null
        TransactionBlock Get(string hash = null);
        void Add(TransactionBlock transaction);
    }
}