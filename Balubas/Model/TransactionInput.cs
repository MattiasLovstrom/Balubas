using System.Diagnostics;
using System.Text;

namespace Balubas.Model
{
    public class TransactionInput : ISigningData
    {
        public string Hash { get; set; }
        public int Row { get; set; }

        public string GetSigningData()
        {
            return $"{Hash}{Row}";
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            var message = new StringBuilder("[");

            message.Append(nameof(Hash)).Append("=").Append(Hash?.Substring(0, 6) ?? "[null]").Append(", ");
            message.Append(nameof(Row)).Append("=").Append(Row);
            message.Append("]");

            return message.ToString();
        }
    }
}