using Balubas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Balubas.Model;
using Balubas.Repositories;

namespace BalubasTests
{
    [TestClass]
    public class IntegrationTests
    {
        private Repository _repository;
        private Wallet _wallet2;
        private Wallet _initialWallet;
        private CryptoHandler _cryptoHandler;
        private Miner _miner;

        [TestInitialize]
        public void Init()
        {
            _cryptoHandler = new CryptoHandler();
            _miner = new Miner(_cryptoHandler){Difficulty = 1};
            ManualSetup();
            _repository = new Repository(_cryptoHandler);

            _initialWallet = new Wallet(_repository, _cryptoHandler)
            {
                FriendlyName = "initial",
                PrivateKey = _initialWallet.PrivateKey,
                PublicKey = _initialWallet.PublicKey
            };
            _wallet2 = new Wallet(_repository, _cryptoHandler) { FriendlyName = "Wallet2" };
            Trace.TraceInformation("Creating wallet2:" + _wallet2);
        }

        private void ManualSetup()
        {
            var genesisWallet = new Wallet(null, _cryptoHandler) { FriendlyName = "Genesis" };
            Genesis.PublicKey = genesisWallet.PublicKey;
            _initialWallet = new Wallet(null, _cryptoHandler) { FriendlyName = "Initial" };
            var transactionOutput = new TransactionOutput
            {
                Amount = Genesis.Amount,
                Receiver = _initialWallet.PublicKey,
                Row = 0
            };
            transactionOutput.Sign = _cryptoHandler.Sign(transactionOutput.GetSigningData(), genesisWallet.PrivateKey);
            Genesis.Block.Outputs = new[] { transactionOutput };
            _miner.Mine(Genesis.Block);
            Genesis.Hash = Genesis.Block.Hash;
            Genesis.Block.Sign = _cryptoHandler.Sign(Genesis.Block.GetSigningData(), genesisWallet.PrivateKey);

            Trace.TraceInformation("Creating initial wallet with public key:" + _initialWallet.PublicKey.Substring(_initialWallet.PublicKey.Length - 6));
            Trace.TraceInformation("Creating Genesis transaction:" + Genesis.Block);
        }

        [TestMethod]
        public void CreateTransactionTest()
        {
            Trace.TraceInformation("Repository:\n" + _repository);

            var transaction = _initialWallet.CreateTransaction(10, _wallet2.PublicKey);
            Trace.TraceInformation("Creating transaction from initial to wallet2:\n" + transaction);
            _miner.Mine(transaction);
            _repository.Add(transaction);
            Trace.TraceInformation("Repository:\n" + _repository);
            Trace.TraceInformation(_initialWallet.ToString());
            Trace.TraceInformation(_wallet2.ToString());
            var wallet3 = new Wallet(_repository, _cryptoHandler) { FriendlyName = "wallet3" };
            Trace.TraceInformation("Creating wallet3:" + wallet3);
            Trace.TraceInformation("Creating to large transaction from wallet2 to wallet3:\n" + transaction);
            try
            {
                transaction = _wallet2.CreateTransaction(11, wallet3.PublicKey);
                _miner.Mine(transaction);
                _repository.Add(transaction);
                Assert.Fail("Should not be able to create transaction for a higher amount then it has");
            }
            catch { }

            Trace.TraceInformation("Repository:\n" + _repository);

            transaction = _wallet2.CreateTransaction(1, wallet3.PublicKey);
            Trace.TraceInformation("Creating transaction from wallet2 to wallet3:\n" + transaction);
            _miner.Mine(transaction);
            _repository.Add(transaction);

            Trace.TraceInformation(wallet3.ToString());



        }
    }
}
