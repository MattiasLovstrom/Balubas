using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Balubas
{
    public class TransactionBlock : IHashData
    {
        public string Hash { get; set; }
        public string PreviousHash { get; set; }
        public ulong Nonce { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        //need to have rowNr
        public IEnumerable<string> Inputs { get; set; }
        public IEnumerable<TransactionOutput> Outputs { get; set; }
        public string Sign { get; set; }


        public string GetHashData()
        {
            var message = new StringBuilder()
                .Append(PreviousHash)
                .Append(Nonce)
                .Append(TimeStamp)
                .Append(Outputs.Select(o=>o.GetHashData()).Aggregate((c, n) => c + "," + n))
                .Append(Inputs != null ? Inputs.Aggregate((c, n) => c + "," + n) : "");

            return message.ToString();
        }

        [DebuggerStepThrough]
        public override string ToString()
        {
            var message = new StringBuilder("[");
            message.Append(nameof(PreviousHash)).Append("=").Append(PreviousHash?.Substring(0, 6) ?? "[null]").Append(", ");
            message.Append(nameof(Hash)).Append("=").Append(Hash?.Substring(0, 6) ?? "[null]").Append(", ");
            if (Inputs != null && Inputs.Any())
            {
                message.Append(nameof(Inputs)).Append("=").Append(Inputs.Select(t => t.Substring(0, 6)).Aggregate((c, n) => c + "," + n)).Append(", ");
            }

            message.Append(nameof(Outputs)).Append("=");
            foreach (var transactionOutput in Outputs)
            {
                message.Append(transactionOutput);
            }
            message.Append(nameof(TimeStamp)).Append("=").Append(TimeStamp.ToString("yyMMdd:Hmm"));
            message.Append("]");
            return message.ToString();
        }
    }
}
