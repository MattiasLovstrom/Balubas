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
        }

        [TestMethod]
        public void AddTest()
        {
            var testObject = new Repository(_cryptoMock.Object);

            var block = TransactionBlockTests.CreateBlock();
            testObject.Add(block);

            Assert.AreSame(block, testObject.First());
        }

        [TestMethod]
        public void GetEnumeratorTest()
        {
            var testObject = new Repository(_cryptoMock.Object);
            Console.Out.WriteLine(testObject.ToString());
            var block1 = TransactionBlockTests.CreateBlock();
            testObject.Add(block1);
            Console.Out.WriteLine(testObject.ToString());
            testObject.Add(TransactionBlockTests.CreateBlock(block1.Hash));
            Console.Out.WriteLine(testObject.ToString());

            Assert.AreEqual(3, testObject.Count());
        }


        [TestMethod]
        public void MyReceivedTest()
        {
            var testObject = new Repository(_cryptoMock.Object);
            var block1 = TransactionBlockTests.CreateBlock();
            block1.Outputs = new[] { new TransactionOutput { Receiver = "myWalletId" } };
            testObject.Add(block1);
            var block2 = TransactionBlockTests.CreateBlock(block1.Hash);
            block2.Outputs = new[] { new TransactionOutput { Receiver = "myWalletId" } };
            testObject.Add(block2);

            Assert.AreEqual(2, testObject.TransactionsTo("myWalletId").Count());
        }

        [TestMethod]
        public void MyUnspentTest()
        {
            var testObject = new Repository(_cryptoMock.Object);
            var transaction1 = TransactionBlockTests.CreateBlock();
            transaction1.Outputs = new [] {new TransactionOutput {Receiver = "myWalletId"}};
            testObject.Add(transaction1);
            var transaction2 = TransactionBlockTests.CreateBlock(transaction1.Hash);
            transaction2.Outputs = new[] { new TransactionOutput { Receiver = "myWalletId" } };
            testObject.Add(transaction2);

            var transactionsTo = testObject.TransactionsTo("myWalletId");
            var unspent = transactionsTo.Where(transaction => !testObject.IsUsed(transaction.Hash));
            Assert.AreEqual(1, unspent.Count());
        }

        [TestMethod]
        public void ValidateInputsTest()
        {
            var testObject = new Repository(_cryptoMock.Object);

            var unspent = TransactionBlockTests.CreateBlock();
            unspent.Outputs = new[] { new TransactionOutput { Amount = 10, Receiver = "PublicKey" } };
            testObject.Add(unspent);

            var spend = TransactionBlockTests.CreateBlock(unspent.Hash);
            spend.Sign = "signature";
            spend.Outputs = new[] { new TransactionOutput { Amount = 10, Receiver = "PublicKeyReceiver" } };
            testObject.Add(spend);

            _cryptoMock.Verify(crypto=>crypto
                .Verify(It.IsAny<string>(), "signature", "PublicKey"),
                "sign must be verified against inputs.Receiver");
        }

        [TestMethod]
        public void SerializeTest()
        {
            var testObject = new Repository(_cryptoMock.Object);
            var json = JsonSerializer.Serialize(testObject);
            Console.Out.WriteLine(json);
        }
    }
}