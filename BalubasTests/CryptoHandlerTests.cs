using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlockChainTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Balubas;

namespace BlockChainTest.Tests
{
    [TestClass()]
    public class CryptoHandlerTests
    {
        [TestMethod]
        public void SignTest()
        {
            var keys = new CryptoHandler().CreatePrivatePublicKeys();
            var testObject = new CryptoHandler();
            var data = "My data";
            var signature = testObject.Sign(data, keys[0]);
            Assert.IsTrue(testObject.Verify(data, signature, keys[1]));
        }

        [TestMethod]
        public void SignWrongDataTest()
        {
            var keys = new CryptoHandler().CreatePrivatePublicKeys();
            var testObject = new CryptoHandler();
            var data = "My data";
            var signature = testObject.Sign(data, keys[0]);
            Assert.IsFalse(testObject.Verify("wrong data", signature, keys[1]));
        }

        [TestMethod]
        public void SignWrongKeyTest()
        {
            var keys = new CryptoHandler().CreatePrivatePublicKeys();
            var testObject = new CryptoHandler();
            var data = "My data";
            var signature = testObject.Sign(data, keys[0]);

            keys = new CryptoHandler().CreatePrivatePublicKeys();

            Assert.IsFalse(testObject.Verify("wrong", signature, keys[1]));
        }

        
        [TestMethod]
        public void ToBase58Test()
        {
            Assert.AreEqual("1MDon6SYQgmZ6uRuK", CryptoHandler.ToBase58(Encoding.UTF8.GetBytes("Hello world!")));

            var fromBase58 = CryptoHandler.FromBase58("1MDon6SYQgmZ6uRuK");
            var fromText = Encoding.UTF8.GetBytes("Hello world!");
            Assert.AreEqual(fromText.Length, fromBase58.Length);
            for (var i = 0; i < fromText.Length; i++)
            {
                Assert.AreEqual(fromText[i], fromBase58[i]);
            }
        }

        [TestMethod]
        public void FromToBaseTest()
        {
            Assert.AreEqual(1 /*15*/, CryptoHandler.FromToBase(new byte[] { 72 }, 256, 58).First());
            Assert.AreEqual(72, CryptoHandler.FromToBase(new byte[] { 1, 14 }, 58, 256).First());
        }

        [TestMethod]
        public void ToBase58SimpleTest()
        {
            Assert.AreEqual("1E", CryptoHandler.ToBase58(new byte[] { 72 }));
            Assert.AreEqual(72, CryptoHandler.FromBase58("1E")[0]);
        }

        [TestMethod]
        public void ToBase58LeadingZeroTest()
        {
            Assert.AreEqual("01E", CryptoHandler.ToBase58(new byte[] { 0, 72 }));
            Assert.AreEqual(0, CryptoHandler.FromBase58("01E")[0]);
        }
    }
}