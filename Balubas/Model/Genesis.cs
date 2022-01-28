using System;

namespace Balubas.Model
{
    public static class Genesis
    {
        public static string PublicKey = "11H8oY0HDAq889AnBbhd0oaGN9nC3goK7q4b23iQervtehWH6NyPHuaAE2EM8dpSjQajXYNHwiudUUPNfdfssQcgxt6EbkSHMWe";
        public static string Hash = "03YyxCFRrxdF9fQ9s6wTromkrNUJRyAnPDoLs0GTm3H0";
        public static string Sign = "53JWTZwfHWo0uLEMamXbJ6Bu08cvfCyGDUF4JoqZFWv4Hs9pQoUJCs2nYjPT46ZZPGrZwqcX2wBG8C70PhaDyZB1";
        public static string TransactionSign = "2SsHM5A8Tq1CuVbPwd9rMUfrjc5527ibCBXDrsD3HrhiXuxQNmx5eAeM969uUXSy0yJug0DvW8Xh4yN5AKKSvT2R";
        public static string TransactionReceiver = "11H8oY0HDAq8kxZVvUQ2SDsS48cpkekBKFo3JUpQcCYT7LjD0njUtCgRk7RSQApvLnxuhewntNtN8KfXav11woy41xvUnB2cEZU";
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
