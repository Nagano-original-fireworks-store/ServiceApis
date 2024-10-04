// ConnectionStatusProvider.cs
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using SharedLibrary;
using static ConnectionStatusNotify.ConnectionStatusProvider;

namespace ConnectionStatusNotify
{
    public class ConnectionStatusProvider : IRouteProvider
    {
        List<uint> UserList = [];
        public Route[] GetRoutes()
        {
            return
            [
                new Route
                {
                    Path = "/bat/game/gameLoginNotify",
                    Method = "POST",
                    Handler = gameLoginNotify
                },
                new Route
                {
                    Path = "/bat/game/gameLogoutNotify",
                    Method = "POST",
                    Handler = gameLogoutNotify
                }
            ];
        }

        private Response gameLoginNotify(HttpListenerRequest request)
        {
            GameLoginNotify gn = new GameLoginNotify();
            string req = Http.Post(request);
            if (req != null) {
                GameLoginNotify _ = (GameLoginNotify)JsonConvert.DeserializeObject(req);
                gn = _;
                Console.WriteLine(gn);
            }
           
            Console.WriteLine(Http.Post(request));
            return Rsp.NewResponse(Rsp.StatusCode.OK, null, Rsp.NewResponseJson(message: "OK"));
        }

        private Response gameLogoutNotify(HttpListenerRequest request)
        {
            string requestBody;
            using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
            {
                requestBody = reader.ReadToEnd();
            }

            Console.WriteLine("收到通知：" + requestBody);

            // 返回JSON响应
            var responseData = new { message = "通知已收到", receivedData = requestBody };
            string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(responseData);
            byte[] buffer = Encoding.UTF8.GetBytes(jsonResponse);

            return new Response
            {
                StatusCode = 200,
                ContentType = "application/json; charset=utf-8",
                Content = buffer
            };
        }

        public struct GameLoginNotify
        {
            [JsonProperty("uid")]
            public uint Uid { get; set; }

            [JsonProperty("account_type")]
            public uint AccountType { get; set; }

            [JsonProperty("account")]
            public uint Account { get; set; }

            [JsonProperty("platform")]
            public uint Platform { get; set; }

            [JsonProperty("region")]
            public string Region { get; set; }

            [JsonProperty("biz_game")]
            public string BizGame { get; set; }
        }
    }
}
