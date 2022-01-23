using System.Diagnostics;
using System.Text;

namespace Balubas
{
    public class TransactionInput : IHashData
    {
        public string Hash { get; set; }
        public int Row { get; set; }

        public string GetHashData()
        {
            return $"{Hash}{Row}";
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            var message = new StringBuilder("[");

            message.Append(nameof(Hash)).Append("=").Append(Hash);
            message.Append(nameof(Row)).Append("=").Append(Row);
            message.Append("]");

            return message.ToString();
        }
    }
}