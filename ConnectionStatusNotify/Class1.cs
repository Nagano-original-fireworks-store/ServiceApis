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
        List<UInt32> UserList = [];
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
                },
                new Route
                {
                    Path = "/bat/game/gameHeartBeatNotify",
                    Method = "POST",
                    Handler = gameHeartBeatNotify
                }
            ];
        }

        private Response gameLoginNotify(HttpListenerRequest request)
        {
            string? req = Http.Post(request);
            if (!string.IsNullOrEmpty(req))
            {
                GameStatusNotify? gn = JsonConvert.DeserializeObject<GameStatusNotify>(req);
                if (gn != null)
                {
                    UserList.Add(gn.Value.Uid);
                }
                else
                {
                    Console.WriteLine("反序列化失败，返回 null。");
                    // 根据需要处理此情况
                }
            }
            else
            {
                Console.WriteLine("请求返回的字符串为 null 或空。");
            }
            return Rsp.NewResponse(Rsp.StatusCode.OK, null, Rsp.NewResponseJson(message: "OK"));
        }

        private Response gameLogoutNotify(HttpListenerRequest request)
        {
            string? req = Http.Post(request);
            if (!string.IsNullOrEmpty(req))
            {
                GameStatusNotify? gn = JsonConvert.DeserializeObject<GameStatusNotify>(req);
                if (gn != null)
                {
                    UserList.Remove(gn.Value.Uid);
                }
                else
                {
                    Console.WriteLine("反序列化失败，返回 null。");
                    // 根据需要处理此情况
                }
            }
            else
            {
                Console.WriteLine("请求返回的字符串为 null 或空。");
            }
            return Rsp.NewResponse(Rsp.StatusCode.OK, null, Rsp.NewResponseJson(message: "OK"));
        }
        private Response gameHeartBeatNotify(HttpListenerRequest request)
        {
            string? req = Http.Post(request);
            if (!string.IsNullOrEmpty(req))
            {
                GameHeartNotify? gh = JsonConvert.DeserializeObject<GameHeartNotify>(req);
                if (gh != null)
                {
                    Console.WriteLine(gh.Value.PlatformUidList.Count);
                    Console.WriteLine(UserList.Count);
                }
                else
                {
                    Console.WriteLine("反序列化失败，返回 null。");
                    // 根据需要处理此情况
                }
            }
            else
            {
                Console.WriteLine("请求返回的字符串为 null 或空。");
            }
            return Rsp.NewResponse(Rsp.StatusCode.OK, null, Rsp.NewResponseJson(message: "OK"));
        }
        public struct GameStatusNotify
        {
            [JsonProperty("uid")]
            public UInt32 Uid { get; set; }

            [JsonProperty("account_type")]
            public uint AccountType { get; set; }

            [JsonProperty("account")]
            public string Account { get; set; }

            [JsonProperty("platform")]
            public uint Platform { get; set; }

            [JsonProperty("region")]
            public string Region { get; set; }

            [JsonProperty("biz_game")]
            public string BizGame { get; set; }
        }
        public struct GameHeartNotify
        {
            [JsonProperty("platform_uid_list")]
            public Dictionary<UInt32, UInt32[]> PlatformUidList { get; set; }

            [JsonProperty("region")]
            public string Region { get; set; }

            [JsonProperty("biz_game")]
            public string BizGame { get; set; }
        }
    }
}
