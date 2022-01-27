using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace Balubas
{
    public class WebRepository : IRepository
    {
        public static string Url = "http://localhost:1050/";
        private readonly Validator _validator;

        public WebRepository(
            ICryptoHandler crypto)
        {
            _validator = new Validator(this, crypto);
        }

        public TransactionBlock Get(string hash = null)
        {
            using var client = new WebClient();
            return JsonSerializer.Deserialize<TransactionBlock>(client.DownloadString(Url + hash));
        }

        public void Add(TransactionBlock transaction)
        {
            // todo make this faster 
            //_validator.Validate(transaction);
            using var client = new WebClient();
            client.UploadString(Url, JsonSerializer.Serialize(transaction));
        }

        public IEnumerator<TransactionBlock> GetEnumerator()
        {
            var current = Get();
            while (current != null)
            {
                yield return current;
                current = Get(current.PreviousHash);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}