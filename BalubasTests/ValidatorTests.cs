using Microsoft.VisualStudio.TestTools.UnitTesting;
using Balubas;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;

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
                PreviousHash = Repository.GenesisBlock.Hash,
                Inputs = new [] { new TransactionInput { Hash = Repository.GenesisBlock.Hash, Row=0} },
                Outputs = new [] { new TransactionOutput { Amount=Repository.GenesisAmount, Receiver = "myPulicKey",Sign="mySign" } },
            };
            
            _transactions = new List<TransactionBlock> 
            { 
                Repository.GenesisBlock
            };

            _repositoryMock = new Mock<IRepository>();
            _repositoryMock
                .Setup(repository => repository.GetEnumerator())
                .Returns(() => _transactions.GetEnumerator());
            _repositoryMock
                .Setup(repository => repository.Get(Repository.GenesisBlock.Hash))
                .Returns(Repository.GenesisBlock);

            _cryptoMock = new Mock<ICryptoHandler>();
            _cryptoMock
                .Setup(crypto => crypto.CalculateHash(It.IsAny<IHashData>()))
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
            _transaction.Hash = "wrong";
            _testObject.Validate(_transaction);
        }

    }
}