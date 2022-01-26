using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once IdentifierTypo
namespace Balubas.Tests
{
    [TestClass]
    public class RepositoryExtensionsTests
    {
        private List<TransactionBlock> _transactions;
        private Mock<IRepository> _repositoryMock;

        [TestInitialize]
        public void Init()
        {

            _transactions = new List<TransactionBlock>();

            _repositoryMock = new Mock<IRepository>();
            _repositoryMock
                .Setup(repository => repository.GetEnumerator())
                .Returns(() => _transactions.GetEnumerator());
            _repositoryMock
                .Setup(repository => repository.Get(Genesis.Hash))
                .Returns(Genesis.Block);
        }

        [TestMethod]
        public void IsUsedTest()
        {
            _transactions.Add(new TransactionBlock
            {
                Hash = "1",
                Outputs = new[]
                {
                    new TransactionOutput { Receiver = "initial" },
                    new TransactionOutput { Receiver = "w1" } }
            });

            var transactionInput = new TransactionInput{ Hash = "1", Row = 1};
            _transactions.Add(new TransactionBlock
            {
                Inputs = new[]
                {
                    transactionInput 
                }
            });


            Assert.IsFalse(_repositoryMock.Object.IsUsed("1", 0));
            Assert.IsTrue(_repositoryMock.Object.IsUsed("1", 1));
        }

        [TestMethod]
        public void TransactionsToSimpleTest()
        {
            _transactions.Add(new TransactionBlock { Outputs = new[] { new TransactionOutput { Receiver = "w1" } } });
            var transactions = _repositoryMock.Object.TransactionsTo("w1");
            Assert.AreEqual(1, transactions.Count());
        }

        [TestMethod]
        public void TransactionsToTest()
        {
            _transactions.Add(new TransactionBlock { Outputs = new[] { new TransactionOutput { Receiver = "w1" } } });
            _transactions.Add(new TransactionBlock { Outputs = new[] { new TransactionOutput { Receiver = "w2" } } });
            var transactions = _repositoryMock.Object.TransactionsTo("w1");
            Assert.AreEqual(1, transactions.Count());
        }

        [TestMethod]
        public void UnspentTransactionsTest()
        {
            _transactions.Add(new TransactionBlock
            {
                Hash = "1",
                Outputs = new[]
                {
                    new TransactionOutput { Receiver = "initial", Row = 0},
                    new TransactionOutput { Receiver = "w1", Row = 1}
                }
            });

            _transactions.Add(new TransactionBlock
            {
                Inputs = new[]
                {
                    new TransactionInput { Hash = "1", Row = 1 }
                },
                Outputs = new[]
                {
                    new TransactionOutput { Receiver = "dummy" },
                }
            });

            Assert.AreEqual(0, _repositoryMock.Object.UnspentTransactions("w1").Count());
            Assert.AreEqual(1, _repositoryMock.Object.UnspentTransactions("initial").Count());
        }
    }
}