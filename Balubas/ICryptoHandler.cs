namespace Balubas
{
    public interface ICryptoHandler
    {
        public string[] CreatePrivatePublicKeys();
        string CalculateHash(TransactionBlock data);
        string Sign(string data, string privateKey);
        public bool Verify(string data, string signature, string publicKey);
    }
}