using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once IdentifierTypo
// ReSharper disable once CheckNamespace
namespace Balubas.Tests
{
    [TestClass()]
    public class SynchronizerTests
    {
        private Mock<IRepository> _repository1Mock;
        private Mock<IRepository> _repository2Mock;
        private Synchronizer _testObject;
        private List<TransactionBlock> _repository1List;
        private List<TransactionBlock> _repository2List;

        [TestInitialize]
        public void Init()
        {
            _repository1Mock = new Mock<IRepository>();
            _repository1List= new List<TransactionBlock>();
            _repository1Mock
                .Setup(repository => repository.GetEnumerator())
                .Returns(() => _repository1List.GetEnumerator());
            _repository1Mock
                .Setup(repository => repository.Get(It.IsAny<string>()))
                .Returns((string hash) => _repository1List.Any(t=>t.Hash == hash) ? new TransactionBlock {Hash = hash} :null);

            _repository2Mock = new Mock<IRepository>();
            _repository2List = new List<TransactionBlock>();
            _repository2Mock
                .Setup(repository => repository.GetEnumerator())
                .Returns(() => _repository2List.GetEnumerator());
            _repository2Mock
                .Setup(repository => repository.Get(It.IsAny<string>()))
                .Returns((string hash) => _repository2List.Any(t => t.Hash == hash) ? new TransactionBlock { Hash = hash } : null);

            _testObject = new Synchronizer(new []{_repository1Mock.Object, _repository2Mock.Object});
        }

        [TestMethod]
        public void SynchronizeBothEmptyTest()
        {
            _testObject.Synchronize();
           
            _repository1Mock.Verify(r1=>r1.Add(It.IsAny<TransactionBlock>()), Times.Never);
            _repository2Mock.Verify(r2=>r2.Add(It.IsAny<TransactionBlock>()), Times.Never);
        }

        [TestMethod]
        public void SynchronizeFirstEmptyTest()
        {
            _repository2List.Add(new TransactionBlock {Hash = "1"});

            _testObject.Synchronize();

            _repository1Mock.Verify(r1 => r1.Add(It.IsAny<TransactionBlock>()), Times.Once);
            _repository2Mock.Verify(r2 => r2.Add(It.IsAny<TransactionBlock>()), Times.Never);
        }

        [TestMethod]
        public void SynchronizeSecondEmptyTest()
        {
            _repository1List.Add(new TransactionBlock { Hash = "1" });

            _testObject.Synchronize();

            _repository1Mock.Verify(r1 => r1.Add(It.IsAny<TransactionBlock>()), Times.Never);
            _repository2Mock.Verify(r2 => r2.Add(It.IsAny<TransactionBlock>()), Times.Once);
        }

        [TestMethod]
        public void Synchronize1_2Test()
        {
            _repository1List.Add(new TransactionBlock { Hash = "1" });
            
            _repository2List.Add(new TransactionBlock { Hash = "2" });
            _repository2List.Add(new TransactionBlock { Hash = "1" });

            _testObject.Synchronize();

            _repository1Mock.Verify(r1 => r1.Add(It.IsAny<TransactionBlock>()), Times.Once);
            _repository2Mock.Verify(r2 => r2.Add(It.IsAny<TransactionBlock>()), Times.Never);
        }


        [TestMethod]
        public void Synchronize3_2Test()
        {
            _repository1List.Add(new TransactionBlock { Hash = "3" });
            _repository1List.Add(new TransactionBlock { Hash = "2" });
            _repository1List.Add(new TransactionBlock { Hash = "1" });

            _repository2List.Add(new TransactionBlock { Hash = "2" });
            _repository2List.Add(new TransactionBlock { Hash = "1" });

            _testObject.Synchronize();

            _repository1Mock.Verify(r1 => r1.Add(It.IsAny<TransactionBlock>()), Times.Never);
            _repository2Mock.Verify(r2 => r2.Add(It.IsAny<TransactionBlock>()), Times.Once);
        }

        [TestMethod]
        public void Synchronize3_1Test()
        {
            _repository1List.Add(new TransactionBlock { Hash = "3" });
            _repository1List.Add(new TransactionBlock { Hash = "2" });
            _repository1List.Add(new TransactionBlock { Hash = "1" });

            _repository2List.Add(new TransactionBlock { Hash = "1" });

            _testObject.Synchronize();

            _repository1Mock.Verify(r1 => r1.Add(It.IsAny<TransactionBlock>()), Times.Never);
            _repository2Mock.Verify(r2 => r2.Add(It.IsAny<TransactionBlock>()), Times.Exactly(2));
        }
    }
}