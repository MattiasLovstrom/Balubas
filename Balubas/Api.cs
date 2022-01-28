using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Balubas
{
    public class Api
    {
        private readonly IRepository _repository;
        private readonly ISynchronizer _synchronizer;

        public HttpListener Listener;
       
        public Api(IRepository repository, ISynchronizer synchronizer)
        {
            _repository = repository;
            _synchronizer = synchronizer;
            Listener = new HttpListener();
            //Listener.Prefixes.Add("http://*:88/");
            Listener.Prefixes.Add("http://localhost:1050/");
            //Listener.Prefixes.Add("http://127.0.0.1:1050/");
            Listener.Start();
            Console.WriteLine("Listening for connections on {0}", WebRepository.Url);
            HandleIncomingConnections();
            Listener.Close();
        }

        public void HandleIncomingConnections()
        {
            while (true)
            {
                Console.Out.WriteLine("Listen");
                var ctx = Listener.GetContext();
                var request = ctx.Request;
                var response = ctx.Response;
                if (request.HttpMethod == "GET")
                {
                    Console.Out.WriteLine("GET: " + request.Url.AbsolutePath);
                    byte[] data;
                    if (_repository.Any())
                    {
                        var hash = request.Url.AbsolutePath.TrimStart('/');
                        data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(_repository.Get(hash), new JsonSerializerOptions { WriteIndented = true }));
                    }
                    else
                    {
                        data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Genesis.Block, new JsonSerializerOptions { WriteIndented = true }));
                    }
                    response.ContentType = "text/html";
                    response.ContentEncoding = Encoding.UTF8;
                    response.ContentLength64 = data.LongLength;
                    response.OutputStream.Write(data, 0, data.Length);
                    response.Close();
                }

                if (request.HttpMethod == "POST")
                {
                    Console.WriteLine("POST:" + request.Url);
                    _repository.Add(JsonSerializer.Deserialize<TransactionBlock>(
                        new StreamReader(request.InputStream).ReadToEnd()));
                    request.InputStream.Close();
                    response.StatusCode = 201;
                    response.Close();
                }

                _synchronizer.Synchronize();
            }
        }
    }
}
