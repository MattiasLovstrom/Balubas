namespace Balubas
{
    public interface IWallet
    {
        string PrivateKey { get; set; }
        string PublicKey { get; set; }
    }
}