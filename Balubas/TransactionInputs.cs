using System.Collections;
using System.Collections.Generic;

namespace Balubas
{
    public class TransactionInputs : IEnumerable<TransactionInput>
    {
        private List<TransactionInput> _inputs = new List<TransactionInput>();

        public IEnumerator<TransactionInput> GetEnumerator()
        {
            return _inputs.GetEnumerator();
        }

        public void Add(TransactionInput input)
        {
            _inputs.Add(input);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}