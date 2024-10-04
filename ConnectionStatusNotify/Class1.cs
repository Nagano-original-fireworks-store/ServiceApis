// ConnectionStatusProvider.cs
using System.Net;
using SharedLibrary;
using Proto.Cmd.Security;
using System.Reflection;

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
                GameLoginNotifyRequest gn = GameLoginNotifyRequest.Parser.ParseJson(req);
                if (gn != null)
                {
                    UserList.Add(gn.Uid);
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
                GameLogoutNotifyRequest gn = GameLogoutNotifyRequest.Parser.ParseJson(req);
                if (gn != null)
                {
                    UserList.Remove(gn.Uid);
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
                GameHeartBeatNotifyRequest gh = GameHeartBeatNotifyRequest.Parser.ParseJson(req);
                if (gh != null)
                {
                    Console.WriteLine($"{Enum.GetName(typeof(Platform), Platform.Android)} Count in {gh.PlatformUidList[(UInt32)Platform.Android].Uid.Count}");
                    Console.WriteLine($"{Enum.GetName(typeof(Platform), Platform.Pc)} Count in {gh.PlatformUidList[(UInt32)Platform.Pc].Uid.Count}");
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
    }
}
