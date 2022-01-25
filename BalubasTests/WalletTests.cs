using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Balubas;
using Moq;

// ReSharper disable once CheckNamespace
namespace BlockChainTest.Tests
{
    [TestClass()]
    public class WalletTests
    {
        private Mock<IRepository> _repositoryMock;
        private Mock<ICryptoHandler> _cryptoMock;

        [TestInitialize]
        public void Init()
        {
            _cryptoMock = new Mock<ICryptoHandler>();
            _cryptoMock
                .Setup(crypto => crypto.CreatePrivatePublicKeys())
                .Returns(new[] { "private", "public" });
            _cryptoMock
                .Setup(crypto => crypto.Verify(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            _repositoryMock = new Mock<IRepository>();
            var transactions = new List<TransactionBlock>
            {
                Genesis.Block,
                new TransactionBlock {Hash = "Hash1", Outputs = new []{new TransactionOutput {Amount = 100, Receiver = "public"}}}
            };
            _repositoryMock
                .Setup(repository => repository.GetEnumerator())
                .Returns(() => transactions.GetEnumerator());
            //_repositoryMock
            //    .Setup(chain => chain.TransactionsTo(It.IsAny<string>()))
            //    .Returns(new List<TransactionBlock>
            //    {
            //        new TransactionBlock {Hash = "Hash1", Outputs = new []{new TransactionOutput {Amount = 100}}}
            //    });
        }

        [TestMethod]
        public void UnspentTransactionTest()
        {
            var wallet = new Wallet(_repositoryMock.Object, _cryptoMock.Object);
            var unspent = wallet.UnspentTransactions;
            Assert.AreEqual(1, unspent.Count());
        }

        [TestMethod]
        public void CreateSimpleTransactionTest()
        {
            var wallet = new Wallet(_repositoryMock.Object, _cryptoMock.Object);
            var wallet2 = new Wallet(_repositoryMock.Object, _cryptoMock.Object);

            var transaction = wallet.CreateTransaction(100, wallet2.PublicKey);
            Assert.AreEqual(1, transaction.Outputs.Length);
            Assert.AreEqual(100.0, transaction.Outputs.First().Amount);
            Assert.AreEqual(1, transaction.Inputs.Length);
            Assert.AreEqual(wallet2.PublicKey, transaction.Outputs.First().Receiver);
        }

        // existing w:100 =>
        // create   w:90, wallet2:10
        [TestMethod]
        public void CreateSplittedTransactionTest()
        {
            var wallet = new Wallet(_repositoryMock.Object, _cryptoMock.Object);

            var transaction = wallet.CreateTransaction(10, "wallet2");
            Assert.AreEqual(2, transaction.Outputs.Length);
            Assert.AreEqual(90.0, transaction.Outputs[0].Amount);
            Assert.AreEqual(10.0, transaction.Outputs[1].Amount);
            Assert.AreEqual("Hash1", transaction.Inputs[0].Hash);
            Assert.AreEqual(wallet.PublicKey, transaction.Outputs[0].Receiver);
            Assert.AreEqual("wallet2", transaction.Outputs[1].Receiver);
        }

        //[TestMethod]
        //public void CreateSpendTransactionTest()
        //{
        //    var wallet = new Wallet(_repositoryMock.Object, _cryptoMock.Object);

        //    var unspent = new TransactionBlock
        //    {
        //        Hash = "hash1",
        //        Amount = 10,
        //        Receiver = wallet.PublicKey,
        //    }; 

        //    _repositoryMock
        //        .Setup(chain => chain.TransactionsTo(It.IsAny<string>()))
        //        .Returns(new List<TransactionBlock> { unspent });
        //    _repositoryMock
        //        .Setup(chain => chain.Get("hash1"))
        //        .Returns(unspent);

        //    var spend = new TransactionBlock
        //    {
        //        Inputs = new []{"hash1"},
        //        Amount = 10,
        //        Receiver = "dummy",
        //        Sign = "signature"
        //    };

        //    // inputs.reviver = walletId
        //    // sign must be verified against inputs.Receiver
        //    Assert.IsTrue(wallet.Verify(spend));



        //}


    }
}