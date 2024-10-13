using SharedLibrary;
using System.Net;

namespace SDK
{
    public class Login : IRouteProvider
    {
        private bool isNeedInit = true;
        public bool IsNeedInit => isNeedInit;
        public void Init() => Inits();
        public Route[] GetRoutes()
        {
            return
            [
                new Route
                {
                    Path = "/hk4e_cn/mdk/shield/api/login",
                    Method = "POST",
                    Handler = Logins
                },
                new Route
                {
                    Path = "/hk4e_global/mdk/shield/api/login",
                    Method = "POST",
                    Handler = Logins
                }
            ];
        }

        private Response Logins(HttpListenerRequest request)
        {

            return null;
        }
        private void Inits()
        {
            var manager = new MySQL();

            // 初始化，传入连接字符串数组
            manager.Init(new string[]
            {
                "Server=localhost;Database=database1;User ID=root;Password=yourpassword;Pooling=true;",
                "Server=localhost;Database=database2;User ID=root;Password=yourpassword;Pooling=true;"
                // 可以添加更多的连接字符串
            });
        }
    }
}
