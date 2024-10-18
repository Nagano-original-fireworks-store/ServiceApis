using MySql.Data.MySqlClient;
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
            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                Port = 3306,
                Database = "Database",
                UserID = "root",
                Password = "Password",
                Pooling = false,
            };
            manager.Init(new string[]
            {
                connectionStringBuilder.ToString(),
            });
        }
    }
}
