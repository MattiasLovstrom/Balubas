using System;

namespace Balubas
{
    public static class Genesis
    {
        public static string PublicKey = "22J9pZ1JEBrDFxLGyCPBy6iqiB1HvtwoBvXW2d8a2zJouSg2PyuLTPrbsJeAY6hUGoMHt538Z4RhbwPCFsm9JV96aUyEWx9oXsc";
        public static string Hash = "BAAug8yCE7ijt5GwyC3BdDxRnhEr4fp8GVgcHQu1QcbN";
        public static string Sign = "4wdugiP65PBeSMVxt6s83qCyaSQ3GsfWAhPT5R49UMWan4S1y3nGVcHLLd3ETLEXz1ZoVBdYS7tLPn13Z1md7UW6";
        public static string TransactionSign = "2VMSKw6gkADDME5Mff1TL2FppmJeYDaKgn9XVKHRzidJP1LC2ZATD7gcSgWQCBgUhdxFT5xf5hwsc7iE2pKdkQmv";
        public static string TransactionReceiver = "22J9pZ1JEBrCRwiAWcUiP1QQefCC2jpA8rq12fyTd93exrn3Kien8FQK7GhCYqyM4Y9rhUJDn1uexRCME5kgjKwmVLJQrQJXZrv"; 
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
                new TransactionOutput { Receiver = TransactionReceiver, Amount = Amount, Row = 0, Sign = TransactionSign }
            },
            Nonce = 0,
            TimeStamp = new DateTime(2022, 01, 21),
            Sign = Sign
        };
    }
}
