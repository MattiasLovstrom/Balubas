using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Balubas.Tests
{
    [TestClass()]
    public class MinerTests
    {
        private Miner _testObject;

        [TestInitialize]
        public void Init()
        {
            _testObject = new Miner(new CryptoHandler()) { Difficulty = 1 };
        }

        [TestMethod()]
        public void MinerTest()
        {
            var transactionBlock = new TransactionBlock
            {
                Inputs = new[]
                {
                    new TransactionInput()
                },
                Outputs = new[]
                {
                    new TransactionOutput()
                }
            };
            _testObject.Mine(transactionBlock);
            Assert.IsTrue(transactionBlock.Hash.StartsWith("0"));
        }

    }
}