using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Balubas
{
    public class TransactionBlock : ISigningData
    {
        public string Hash { get; set; }
        public string PreviousHash { get; set; }
        public ulong Nonce { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public TransactionInput[] Inputs { get; set; }
        public TransactionOutput[] Outputs { get; set; }
        public string Sign { get; set; }


        public string GetSigningData()
        {
            var message = new StringBuilder()
                .Append(PreviousHash)
                .Append(TimeStamp)
                .Append(Outputs.Select(o=>o.GetSigningData()).Aggregate((c, n) => c + "," + n))
                .Append(Inputs.Select(o=>o.GetSigningData()).Aggregate((c, n) => c + "," + n));

            return message.ToString();
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            var message = new StringBuilder("[");
            message.Append(nameof(PreviousHash)).Append("=").Append(PreviousHash?.Substring(0, 6) ?? "[null]").Append(", ");
            message.Append(nameof(Hash)).Append("=").Append(Hash?.Substring(0, 6) ?? "[null]").Append(", ");
            message.Append(nameof(Inputs)).Append("=");
            foreach (var transactionInput in Inputs)
            {
                message.Append(transactionInput);
            }
            message.Append(", ");
            message.Append(nameof(Outputs)).Append("=");
            foreach (var transactionOutput in Outputs)
            {
                message.Append(transactionOutput);
            }

            message.Append("]");
            return message.ToString();
        }
    }
}
