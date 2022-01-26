using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Text.Json;
using Balubas;

// ReSharper disable once CheckNamespace
namespace BlockChainTest.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        private Mock<ICryptoHandler> _cryptoMock;
        private Mock<IValidator> _validatorMock;
        private Repository _testObject;

        [TestInitialize]
        public void Init()
        {
            _cryptoMock = new Mock<ICryptoHandler>();
            _cryptoMock
                .Setup(crypto => crypto.CalculateHash(It.IsAny<TransactionBlock>()))
                .Returns((TransactionBlock h) => h.GetHashCode().ToString());
            _cryptoMock
                .Setup(crypto => crypto.Verify(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            _validatorMock = new Mock<IValidator>();
            _testObject = new Repository(
                _cryptoMock.Object,
                _validatorMock.Object);
        }

        [TestMethod]
        public void AddTest()
        {
            var block = new TransactionBlock { Hash = "1" };
            _testObject.Add(block);

            Assert.AreSame(block, _testObject.First());
        }

        [TestMethod]
        public void GetEnumeratorTest()
        {
            var block1 = new TransactionBlock { Hash = "1" };
            _testObject.Add(block1);
            _testObject.Add(new TransactionBlock { PreviousHash = "1", Hash = "2" });

            Assert.AreEqual(2, _testObject.Count());
        }


        [TestMethod]
        public void MyReceivedTest()
        {
            var block1 = new TransactionBlock { Hash = "1" };
            block1.Outputs = new[] { new TransactionOutput { Amount = 1, Receiver = "myWalletId" } };
            _testObject.Add(block1);
            var block2 = new TransactionBlock { PreviousHash = "1", Hash = "2" };
            block2.Outputs = new[] { new TransactionOutput { Amount = 2, Receiver = "myWalletId" } };
            _testObject.Add(block2);
            var block3 = new TransactionBlock { PreviousHash = "2", Hash = "3" };
            block3.Outputs = new[] { new TransactionOutput { Amount = 2, Receiver = "another" } };
            _testObject.Add(block3);

            Assert.AreEqual(2, _testObject.TransactionsTo("myWalletId").Count());
        }

        [TestMethod]
        public void MyUnspentTest()
        {
            var transaction1 = new TransactionBlock { Hash = "1" };
            transaction1.Outputs = new[] { new TransactionOutput { Amount = 1, Receiver = "myWalletId" } };
            _testObject.Add(transaction1);
            var transaction2 = new TransactionBlock { PreviousHash = "1", Hash = "2" };
            transaction2.Inputs = new [] { new TransactionInput { Hash = "1", Row = 0 } };
            transaction2.Outputs = new[] { new TransactionOutput { Amount = 2, Receiver = "myWalletId" } };
            _testObject.Add(transaction2);

            var unspent = _testObject.UnspentTransactions("myWalletId");
            Assert.AreEqual(1, unspent.Count());
        }

        //[TestMethod]
        //public void ValidateInputsTest()
        //{
        //    var testObject = new Repository(_cryptoMock.Object);

        //    var unspent = TransactionBlockTests.CreateBlock();
        //    unspent.Inputs = new TransactionInputs { new TransactionInput { Hash = Repository.GenesisBlockPublicKey, Row = 0} };
        //    unspent.Outputs = new[] { new TransactionOutput { Amount = 10, Receiver = "PublicKey" } };
        //    testObject.Add(unspent);

        //    var spend = TransactionBlockTests.CreateBlock(unspent.Hash);
        //    spend.Sign = "signature";
        //    spend.Outputs = new[] { new TransactionOutput { Amount = 10, Receiver = "PublicKeyReceiver" } };
        //    testObject.Add(spend);

        //    _cryptoMock.Verify(crypto=>crypto
        //        .Verify(It.IsAny<string>(), "signature", "PublicKey"),
        //        "sign must be verified against inputs.Receiver");
        //}
    }
}