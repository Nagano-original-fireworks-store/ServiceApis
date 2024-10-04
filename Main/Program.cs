// Program.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using SharedLibrary;

namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            string[] prefixes = { "http://localhost:5000/" };
            HttpListener listener = new HttpListener();

            foreach (string prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }

            // 加载路由
            List<Route> routes = LoadRoutes();

            // **在此处打印注册的路由信息**
            Console.WriteLine("已注册的路由信息：");
            foreach (var route in routes)
            {
                Console.WriteLine($"{route.Method} {route.Path} -----> {route.HandlerName}");
            }

            // 启动监听
            listener.Start();
            Console.WriteLine("HTTP服务器已启动...");

            // 开始处理请求
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HandleRequest(context, routes);
            }
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Console.WriteLine($"异常类型：{ex.GetType().Name}");
            Console.WriteLine($"异常消息：{ex.Message}");
            Console.WriteLine($"堆栈跟踪：{ex.StackTrace}");
            // Environment.Exit(1);
        }
        static List<Route> LoadRoutes()
        {
            List<Route> routes = new List<Route>();

            // 动态加载DLL
            string pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            if (!Directory.Exists(pluginsDirectory))
            {
                Directory.CreateDirectory(pluginsDirectory);
            }

            string[] dllFiles = Directory.GetFiles(pluginsDirectory, "*.dll");

            foreach (string dll in dllFiles)
            {
                Assembly assembly = Assembly.LoadFrom(dll);
                var types = assembly.GetTypes().Where(t => typeof(IRouteProvider).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var type in types)
                {
                    object? instance = Activator.CreateInstance(type);
                    if (instance is IRouteProvider provider)
                    {
                        routes.AddRange(provider.GetRoutes());
                    }
                    else
                    {
                        // 处理无法创建实例的情况，记录日志或抛出异常
                        Console.WriteLine($"无法创建类型 {type.FullName} 的实例。");
                    }

                }
            }

            return routes;
        }

        static void HandleRequest(HttpListenerContext context, List<Route> routes)
        {
            string requestPath = context.Request.Url?.AbsolutePath ?? "/";
            string requestMethod = context.Request.HttpMethod;

            // 查找匹配的路由
            var route = routes.FirstOrDefault(r =>
                r.Path.Equals(requestPath, StringComparison.OrdinalIgnoreCase) &&
                r.Method.Equals(requestMethod, StringComparison.OrdinalIgnoreCase));

            if (route != null)
            {
                // 调用处理函数，获取Response对象
                Response response = route.Handler(context.Request);

                // 设置响应状态码
                context.Response.StatusCode = response.StatusCode;

                // 设置响应头
                context.Response.ContentType = response.ContentType;
                foreach (var header in response.Headers)
                {
                    context.Response.Headers[header.Key] = header.Value;
                }

                // 写入响应内容
                if (response.Content != null)
                {
                    context.Response.ContentLength64 = response.Content.Length;
                    context.Response.OutputStream.Write(response.Content, 0, response.Content.Length);
                }

                context.Response.OutputStream.Close();
            }
            else
            {
                // 返回404未找到
                context.Response.StatusCode = 404;
                context.Response.Close();
            }
        }
    }
}
