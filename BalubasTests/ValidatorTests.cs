using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using Balubas.Model;
using Balubas.Repositories;

// ReSharper disable once IdentifierTypo
namespace Balubas.Tests
{
    [TestClass()]
    public class ValidatorTests
    {
        private TransactionBlock _transaction;
        private List<TransactionBlock> _transactions;
        private Mock<IRepository> _repositoryMock;
        private Mock<ICryptoHandler> _cryptoMock;
        private Validator _testObject;

        [TestInitialize]
        public void Init()
        {
            _transaction = new TransactionBlock
            {
                Hash = "1",
                PreviousHash = Genesis.Hash,
                Inputs = new[] { new TransactionInput { Hash = Genesis.Hash, Row = 0 } },
                Outputs = new[] { new TransactionOutput { Amount = Genesis.Amount, Receiver = "myPulicKey", Sign = "mySign" } },
                Sign = "mySign"
            };

            _transactions = new List<TransactionBlock>
            {
                Genesis.Block
            };

            _repositoryMock = new Mock<IRepository>();
            _repositoryMock
                .Setup(repository => repository.GetEnumerator())
                .Returns(() => _transactions.GetEnumerator());
            _repositoryMock
                .Setup(repository => repository.Get(Genesis.Hash))
                .Returns(Genesis.Block);

            _cryptoMock = new Mock<ICryptoHandler>();
            _cryptoMock
                .Setup(crypto => crypto.CalculateHash(It.IsAny<TransactionBlock>()))
                .Returns("1");
            _cryptoMock
                .Setup(crypto => crypto.Verify(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            _testObject = new Validator(
                _repositoryMock.Object,
                _cryptoMock.Object);
        }

        [TestMethod]
        public void ValidateTest()
        {
            _testObject.Validate(_transaction);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ValidateWrongPreviousHashTest()
        {
            _transaction.PreviousHash = "wrong";
            _testObject.Validate(_transaction);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ValidateWrongHashTest()
        {
            _transaction.Hash = "";
            _testObject.Validate(_transaction);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ValidateWrongReferredInputTest()
        {
            _transaction.Inputs = new[] { new TransactionInput { Hash = "wrong", Row = 0 } };
            _testObject.ValidateInputs(_transaction);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ValidateWrongSignedInputTest()
        {
            _cryptoMock
               .Setup(crypto => crypto.Verify(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
               .Returns(false);
            _testObject.ValidateInputs(_transaction);
        }


    }
}