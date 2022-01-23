using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Balubas
{
    public class Program
    {
        private static CryptoHandler _crypto;
        private static Repository _repository;

        static void Main(string[] args)
        {
            _crypto = new CryptoHandler();
            _repository = new Repository(_crypto);
            if (!args.Any()) return;

            if (args[0] == "createwallet" && args.Length == 2) CreateWallet(args[1]);
            else
            {

                Console.Out.WriteLine("Usage:");
                Console.Out.WriteLine("   createwallet <friendly name> - creates a new wallet");
            }
        }

        private static void CreateWallet(string name)
        {
            if (string.IsNullOrEmpty(name?.Trim(' '))) throw new ApplicationException("Can't create a wallet without name.");
            var wallet = new Wallet(_repository, _crypto);
            var keys = _crypto.CreatePrivatePublicKeys();
            wallet.PrivateKey = keys[0];
            wallet.PublicKey = keys[1];
            var fileName = $"{name}.wallet";
            if (File.Exists(fileName)) throw new ApplicationException($"Wallet already exists {fileName}");
            File.WriteAllText(fileName, JsonSerializer.Serialize(wallet));
            Console.Out.WriteLine($"Created wallet: {fileName}");
        }

        //static void Test()
        //{
        //    var crypto = new CryptoHandler();

        //    var repository = new Repository(crypto);
        //    Console.Out.WriteLine("Repository\n" + repository);
        //    var rootWallet = new Wallet(repository, crypto) { FriendlyName = "root" };
        //    Console.Out.WriteLine("Adding root wallet");
        //    Console.Out.WriteLine(rootWallet);
        //    repository.Add(new TransactionBlock
        //    {
        //        Outputs = new []{new TransactionOutput{Amount = 10, Receiver = rootWallet.PublicKey}}
        //    });
        //    Console.Out.WriteLine("Adding initial transaction");
        //    Console.Out.WriteLine("Repository\n" + repository);

        //    var wallet1 = new Wallet(repository, crypto) { FriendlyName = "Wallet 1" };
        //    Console.Out.WriteLine(wallet1);
        //    var wallet2 = new Wallet(repository, crypto) { FriendlyName = "Wallet 2" };
        //    Console.Out.WriteLine(wallet2);

        //    var transactions1 = rootWallet.Send(1, wallet1.PublicKey);
        //    Console.Out.WriteLine("Send: rootWallet.Send(1, wallet1)\n" + transactions1.Select(t => t.ToString()).Aggregate((c, n) => c + "\n" + n));
        //    foreach (var transactionBlock in transactions1)
        //    {
        //        repository.Add(transactionBlock);
        //    }
        //    Console.Out.WriteLine("Repository\n" + repository);
        //    var transactions2 = rootWallet.Send(2, wallet2.PublicKey);
        //    Console.Out.WriteLine("Send: rootWallet.Send(2, wallet2)\n" + transactions2.Select(t => t.ToString()).Aggregate((c, n) => c + "\n" + n));
        //    foreach (var transactionBlock in transactions2)
        //    {
        //        repository.Add(transactionBlock);
        //    }
        //    Console.Out.WriteLine("Repository\n" + repository);
        //    Console.WriteLine($"{rootWallet.FriendlyName}: {rootWallet.UnspentTransactions.Sum(t => t.Amount)}");
        //    Console.WriteLine($"{wallet1.FriendlyName}: {wallet1.UnspentTransactions.Sum(t => t.Amount)}");
        //    Console.WriteLine($"{wallet2.FriendlyName}: {wallet2.UnspentTransactions.Sum(t => t.Amount)}");
        //}
    }
}
