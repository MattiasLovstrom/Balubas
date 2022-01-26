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
            Assert.IsFalse(block.GetSigningData().Contains("myHash"));
        }

        public static TransactionBlock CreateBlock(string previousHash = null)
        {
            previousHash ??= Genesis.Hash;
            var block = new TransactionBlock();
            block.Hash = block.GetHashCode().ToString();
            block.PreviousHash = previousHash;
            block.Inputs = new[] { new TransactionInput { Hash = previousHash, Row = 0 } };
            block.Outputs = new[] { new TransactionOutput { Amount = Genesis.Amount, Receiver = "myPublicKey", Sign = "MySign" } };

            return block;
        }
    }
}