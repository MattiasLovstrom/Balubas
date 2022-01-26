using System.Diagnostics;
using System.Text;

namespace Balubas
{
    public class TransactionOutput : IHashData
    {
        public double Amount { get; set; }
        public string Receiver { get; set; }
        public string Sign { get; set; }
        public int Row { get; set; }

        public string GetHashData()
        {
            return $"{Amount}{Receiver}{Sign}";
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            var message = new StringBuilder("[");
            message.Append(nameof(Amount)).Append("=").Append(Amount).Append(", ");
            message.Append(nameof(Receiver)).Append("=").Append(Receiver?.Substring(Receiver.Length - 6) ?? "[null]").Append(", ");
            message.Append(nameof(Row)).Append("=").Append(Row);
            message.Append("]");
            return message.ToString();
        }
    }
}