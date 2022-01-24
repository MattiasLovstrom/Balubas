using System;

namespace Balubas
{
    public static class Genesis
    {
        public static string Hash = "0000000000000000000";
        public static string PublicKey = "22J9pZ1JEBrDxhrdojPUEc4aQPz7mAHmJN9vAL7xN41QTD2HYPne2Trus6j3CDQe6safsAZk9WEkXN1Xjhxh65X22cZLAP2uuMp";
        public const double Amount = 1000000;

        public static readonly TransactionBlock Block = new TransactionBlock
        {
            PreviousHash = null,
            Hash = Hash,
            Inputs = new[]
            {
                new TransactionInput{ Hash = "-------", Row = 0}
            },
            Outputs = new[]
            {
                new TransactionOutput { Receiver = PublicKey, Amount = Amount, Sign = "" }
            },
            Nonce = 0,
            TimeStamp = new DateTime(2022, 01, 21),
            Sign = ""
        };
    }
}
