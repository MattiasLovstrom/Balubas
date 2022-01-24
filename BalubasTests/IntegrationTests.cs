using System;
using System.Collections.Generic;
using System.Text;
using Balubas;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BalubasTests
{
    [TestClass]
    public class IntegrationTests
    {
        private Repository _repository;
        private Wallet _wallet2;
        private Wallet _initialWallet;
        private CryptoHandler _cryptoHandler;

        [TestInitialize]
        public void Init()
        {
            _cryptoHandler = new CryptoHandler();
            ManualSetup();
            _repository = new Repository(_cryptoHandler);

            _initialWallet = new Wallet(_repository, _cryptoHandler) 
            {
                PrivateKey = _initialWallet.PrivateKey, 
                PublicKey = _initialWallet.PublicKey
            };
            _wallet2 = new Wallet(_repository, _cryptoHandler) { FriendlyName = "Wallet 2" };
        }

        private void ManualSetup()
        {
            var genesisWallet = new Wallet(null, _cryptoHandler) {FriendlyName = "Genesis"};
            Genesis.PublicKey = genesisWallet.PublicKey;
            _initialWallet = new Wallet(null, _cryptoHandler) {FriendlyName = "Initial"};
            var transactionOutput = new TransactionOutput
            {
                Amount = Genesis.Amount,
                Receiver = _initialWallet.PublicKey
            };
            transactionOutput.Sign = _cryptoHandler.Sign(transactionOutput.GetHashData(), genesisWallet.PrivateKey);
            Genesis.Block.Outputs = new[] {transactionOutput};
            Genesis.Block.Hash = Genesis.Hash = _cryptoHandler.CalculateHash(Genesis.Block);
            Genesis.Block.Sign = _cryptoHandler.Sign(Genesis.Block.GetHashData(), genesisWallet.PrivateKey);
        }

        [TestMethod]
        public void SendTest()
        {
            Console.Out.WriteLine("Repository:\n" + _repository);
            var transaction = _initialWallet.CreateTransaction(10, _wallet2.PublicKey);
            Console.Out.WriteLine("Transaction:\n" + transaction);
            _repository.Add(transaction);
            Console.Out.WriteLine("Repository:\n" + _repository);
        }
    }
}
