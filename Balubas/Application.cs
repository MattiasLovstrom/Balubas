using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Balubas
{
    public class Application
    {
        private readonly ICryptoHandler _crypto;
        private readonly IRepository _repository;
        private readonly IRepository _localStorage;
        private readonly ISynchronizer _synchronizer;
        private readonly WebRepository _webStorage;

        public Application()
        {
            _crypto = new CryptoHandler();
            _repository = new Repository(_crypto);
            _localStorage = new FileRepository(_crypto);
            var repositories = new List<IRepository> { _repository, _localStorage};
            try
            {
                _webStorage = new WebRepository(_crypto);
                var _ = _webStorage.First();
                repositories.Add(_webStorage);
            }
            catch
            {
                Console.Out.WriteLine($"Can't connect to to web repository {WebRepository.Url}");
            }

            _synchronizer = new Synchronizer(repositories);
        }

        public Application(
            ICryptoHandler cryptoHandler, 
            IRepository repository,
            IRepository localStorage, 
            ISynchronizer synchronizer)
        {
            _crypto = cryptoHandler;
            _repository = repository;
            _localStorage = localStorage;
            _synchronizer = synchronizer;
        }
        
        public void WalletBalance(string walletFriendlyName = null)
        {
            _synchronizer.Synchronize();
            var myWallet = LoadWallet(walletFriendlyName);
            Console.Out.WriteLine("Balance: " + _repository.UnspentTransactions(myWallet.PublicKey));
        }

        public void Send(string walletFriendlyName, string toPublicKey, string amountString)
        {
            _synchronizer.Synchronize();

            if (!double.TryParse(amountString, NumberStyles.Any, CultureInfo.InvariantCulture, out double amount)) throw new ApplicationException($"can't parse {amountString} to a double.");
            var wallet = LoadWallet(walletFriendlyName);
            if (!_repository.TransactionsTo(toPublicKey).Any())
            {
                Console.Out.WriteLine($"Are you sure you want to send transaction to the never used anonymous wallet '{toPublicKey}' ?");
                var answer = (char)Console.In.Read();
                if (!(answer == 'Y' || answer == 'y'))
                {
                    throw new ApplicationException("Transaction stopped by user.");
                }
            }

            var transaction = wallet.CreateTransaction(amount, toPublicKey);
            _repository.Add(transaction);
        
            _synchronizer.Synchronize();
        }

        private Wallet LoadWallet(string walletFriendlyName = null)
        {
            if (string.IsNullOrEmpty(walletFriendlyName))
            {
                walletFriendlyName = "*";
            }
            var wallets = Directory.EnumerateFiles(".", $"{walletFriendlyName}.wallet").ToArray();
            if (!wallets.Any()) throw new ApplicationException("Can't find a *.wallet file in current directory. (use 'createwallet' argument to create a wallet) ");
            if (wallets.Length > 1) throw new ApplicationException("Find multiple .wallet files in current directory.");

            var walletInfo = JsonSerializer.Deserialize<Wallet>(File.ReadAllText(wallets.First()));
            var wallet = new Wallet(_repository, _crypto)
            {
                PrivateKey = walletInfo.PrivateKey,
                PublicKey = walletInfo.PublicKey,
                FriendlyName = new FileInfo(wallets.First()).Name.Replace(".wallet", "")
            };
            Console.Out.WriteLine($"Using wallet ({wallet.FriendlyName}):{wallet.PublicKey}");

            return wallet;
        }

        public void CreateWallet(string friendlyName)
        {
            if (string.IsNullOrEmpty(friendlyName?.Trim(' '))) throw new ApplicationException("Can't create a wallet without name.");
            var wallet = new Wallet(_repository, _crypto);
            var keys = _crypto.CreatePrivatePublicKeys();
            wallet.PrivateKey = keys[0];
            wallet.PublicKey = keys[1];
            var fileName = $"{friendlyName}.wallet";
            if (File.Exists(fileName)) throw new ApplicationException($"Wallet already exists {fileName}");
            File.WriteAllText(fileName, JsonSerializer.Serialize(wallet, new JsonSerializerOptions { WriteIndented = true }));
            Console.Out.WriteLine($"Created wallet: {fileName}");
        }

        public void CreateGenesis()
        {
            var genesisWallet = new Wallet(_repository, _crypto);
            CreateWallet("initial");
            var initialWallet = LoadWallet();

            var transactionOutput = new TransactionOutput
            {
                Amount = Genesis.Amount,
                Receiver = initialWallet.PublicKey
            };
            transactionOutput.Sign = _crypto.Sign(transactionOutput.GetSigningData(), genesisWallet.PrivateKey);
            Genesis.Block.Outputs = new[] { transactionOutput };
            Genesis.Block.Hash = Genesis.Hash = _crypto.CalculateHash(Genesis.Block);
            Genesis.Block.Sign = _crypto.Sign(Genesis.Block.GetSigningData(), genesisWallet.PrivateKey);

            Console.Out.WriteLine($"public static string {nameof(Genesis.PublicKey)} = \"{genesisWallet.PublicKey}\";");
            Console.Out.WriteLine($"public static string {nameof(Genesis.Hash)} = \"{Genesis.Hash}\";");
            Console.Out.WriteLine($"public static string {nameof(Genesis.Sign)} = \"{Genesis.Block.Sign}\";");
            Console.Out.WriteLine($"public static string {nameof(Genesis.TransactionSign)} = \"{transactionOutput.Sign}\";");
            Console.Out.WriteLine($"public static string {nameof(Genesis.TransactionReceiver)} = \"{initialWallet.PublicKey}\";");

            Console.Out.WriteLine(JsonSerializer.Serialize(Genesis.Block, new JsonSerializerOptions { WriteIndented = true }));
        }

        public void StartServer()
        {
            new Api(_repository, new Synchronizer(new[] { _repository, _localStorage}));
        }
    }
}