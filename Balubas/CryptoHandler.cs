using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Balubas
{
    public class CryptoHandler : ICryptoHandler
    {
        private readonly byte[] _hashKey = new byte[64];

        public string CalculateHash(IHashData data)
        {
            using var hmac = new HMACSHA256(_hashKey);
            var hashString = ToBase58(
                hmac.ComputeHash(
                    Encoding.UTF8.GetBytes(data.GetHashData())));
            return hashString;
        }

        public string[] CreatePrivatePublicKeys()
        {
            var keyCreationParameters = new CngKeyCreationParameters
            {
                ExportPolicy = CngExportPolicies.AllowPlaintextExport,
                KeyUsage = CngKeyUsages.Signing
            };

            var dsa = new ECDsaCng(CngKey.Create(CngAlgorithm.ECDsaP256, null, keyCreationParameters));             
            return new[]
            {
                ToBase58(dsa.Key.Export(CngKeyBlobFormat.EccPrivateBlob)),
                ToBase58(dsa.Key.Export(CngKeyBlobFormat.EccPublicBlob))
            };
        }

        public string Sign(string data, string privateKey)
        {
            Console.Out.WriteLine("Sign:" + data);
            var key = CngKey.Import(FromBase58(privateKey), CngKeyBlobFormat.EccPrivateBlob);
            var dsa = new ECDsaCng(key); 
            
            return ToBase58(dsa.SignData(Encoding.UTF8.GetBytes(data)));
        }

        public bool Verify(string data, string signature, string publicKey)
        {
            Console.Out.WriteLine("Verify:" + data);
            var key = CngKey.Import(FromBase58(publicKey), CngKeyBlobFormat.EccPublicBlob);
            var dsa = new ECDsaCng(key); 

            return dsa.VerifyData(Encoding.UTF8.GetBytes(data), FromBase58(signature));
        }

        private static readonly char[] Base58Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();

        public static string ToBase58(byte[] bytes)
        {
            return new string(FromToBase(bytes, 256, 58).Select(b=> Base58Alphabet[b]).ToArray());
        }

        public static byte[] FromBase58(string base58)
        {
            var alphabet = Base58Alphabet.ToList();
            var base58bytes = base58.Select(b => (byte)(alphabet.IndexOf(b))).ToArray();
            return FromToBase(base58bytes, 58, 256).ToArray();
        }

        public static IEnumerable<byte> FromToBase(byte[] bytes, int fromBase, int toBase)
        {
            var num = new BigInteger();
            var leadingZero = true;
            for (var i = 0; i < bytes.Length; i++)
            {
                if (leadingZero)
                {
                    if (bytes[i] == 0) yield return 0;
                    else leadingZero = false;
                }

                num = BigInteger.Add(num, BigInteger.Multiply(bytes[i], BigInteger.Pow(fromBase, bytes.Length - i - 1)));
            }
            for (var i = (int)BigInteger.Log(num, toBase); i >= 0; i--)
            {
                var b = (byte)BigInteger.Divide(num, BigInteger.Pow(toBase, i));
                yield return b;
                num -= b * BigInteger.Pow(toBase, i);
            }
        }
    }
}
