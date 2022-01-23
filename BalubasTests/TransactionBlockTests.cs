using Balubas;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable once CheckNamespace
namespace BlockChainTest.Tests
{
    [TestClass]
    public class TransactionBlockTests
    {
        [TestMethod]
        public void GetHashDataTest()
        {
            var block = TransactionBlockTests.CreateBlock();
            block.Hash = "myHash";
            Assert.IsFalse(block.GetHashData().Contains("myHash"));
        }

        public static TransactionBlock CreateBlock(string previousHash = Repository.GenesisHash)
        {
            var block = new TransactionBlock();
            block.Hash = block.GetHashCode().ToString();
            block.PreviousHash = previousHash;
            block.Inputs = new[] { new TransactionInput {Hash = previousHash, Row = 0}};
            block.Outputs = new[] {new TransactionOutput {Amount = Repository.GenesisAmount, Receiver = "myPublicKey", Sign = "MySign"}};
            
            return block;
        }
    }
}