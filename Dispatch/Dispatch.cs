using System.Net;
using SharedLibrary;

namespace Dispatch
{
    public class Dispatch : IRouteProvider
    {
        private bool isNeedInit = false;
        public bool IsNeedInit => isNeedInit;
        public void Init() { }
        public Route[] GetRoutes()
        {
            return
            [
                new Route
                {
                    Path = "/hk4e_cn/mdk/shield/api/login",
                    Method = "POST",
                    Handler = Dispatchs
                },
                new Route
                {
                    Path = "/hk4e_global/mdk/shield/api/login",
                    Method = "POST",
                    Handler = Dispatchs
                }
            ];
        }

        private Response Dispatchs(HttpListenerRequest request)
        {

            return null;
        }
    }
}
