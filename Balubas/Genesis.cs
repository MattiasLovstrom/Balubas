using System;

namespace Balubas
{
    public static class Genesis
    {
        public static string PublicKey = "22J9pZ1JEBrAeNCNyzQVSxwkX1hozgoAdYUo1CdiBF49RYeimcLbUTMjEkoqUmPqAmdDm3rThjykLAw5HHHq6pkLsCbgwhEXesR";
        public static string Hash = "Agq3oQo9kJWXdhBYbMLFmUCByK8hfK7c9cg3Y6GYvxkr";
        public static string Sign = "2usnLAWr1MkW8Y9o69GEK7xkunUgGPZXpFuNF4a55H8mNE4CWuZ53iGBwLrjiGoyHhVQDzBCrZ393DU4jXCePeRC";
        public static string TransactionSign = "2UunEyJB5gbuqFfVYYT7VAyg1b953NKW2M47yEPXDnv4AEiRBamgECyu1Zm2nQVxBM4ESqfRuDdC6p6yuLHLKVqV";
        public static string TransactionReceiver = "22J9pZ1JEBrCjHxZzBEi2Erj3tQxKTv7KMynYPa1C9hnn9xTySLAu7ps9xYs6EPXwC39qTuAQ1BAqMQYSjGMTenCx6EKf67X3wT"; public const double Amount = 1000000;

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
                new TransactionOutput { Receiver = TransactionReceiver, Amount = Amount, Sign = TransactionSign }
            },
            Nonce = 0,
            TimeStamp = new DateTime(2022, 01, 21),
            Sign = Sign
        };
    }
}
