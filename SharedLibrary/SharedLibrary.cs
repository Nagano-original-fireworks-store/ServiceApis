// Response.cs
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;

namespace SharedLibrary
{
    public struct ServiceApiResponse
    {
        [JsonProperty("data")]
        public object? Data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("retcode")]
        public uint Retcode { get; set; }
    }

    public class Response
    {
        public int StatusCode { get; set; } = 200; // 默认状态码200 OK
        public string ContentType { get; set; } = "text/plain"; // 默认内容类型
        public byte[]? Content { get; set; } // 响应内容
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>(); // 可选的响应头
    }

    public static class Rsp
    {
        public enum StatusCode
        {
            Continue = 100,
            SwitchingProtocols,
            Processing,
            EarlyHints,
            OK = 200,
            Created,
            Accepted,
            NonAuthoritativeInformation,
            NoContent,
            ResetContent,
            PartialContent,
            MultiStatus,
            AlreadyReported,
            MultipleChoices = 300,
            MovedPermanently,
            Found,
            SeeOther,
            NotModified,
            UseProxy,
            Unused,
            TemporaryRedirect,
            BadRequest = 400,
            Unauthorized,
            PaymentRequired,
            Forbidden,
            NotFound,
            MethodNotAllowed,
            NotAcceptable,
            ProxyAuthenticationRequired,
            RequestTimeOut,
            Conflict,
            Gone,
            LengthRequired,
            PreconditionFailed,
            RequestEntityTooLarge,
            RequestURITooLarge,
            UnsupportedMediaType,
            RequestedRangeNotSatisfiable,
            ExpectationFailed,
            InternalServerError = 500,
            NotImplemented,
            BadGateway,
            ServiceUnavailable,
            GatewayTimeOut,
            HTTPVersionNotSupported,
        }

        public struct ResponseJson
        {
            [JsonProperty("data")]
            public object? Data { get; set; }

            [JsonProperty("message")]
            public string? Message { get; set; }

            [JsonProperty("retcode")]
            public uint Retcode { get; set; }
        }

        public static Response NewResponse(StatusCode StatusCode, string? ContentType, object? Content)
        {
            if ((int)StatusCode >= 100 && (int)StatusCode <= 999)
            {
                // 状态码合法
            }
            else
            {
                // 状态码不在 100 到 999 之间
                throw new ArgumentException("状态码必须为三位数！");
            }

            byte[] contentBytes;
            if (Content != null)
            {
                string jsonString = JsonConvert.SerializeObject(Content);
                contentBytes = Encoding.UTF8.GetBytes(jsonString);
            }
            else
            {
                contentBytes = Array.Empty<byte>();
            }

            // 如果未指定 ContentType，则设置为 application/json
            if (string.IsNullOrEmpty(ContentType))
            {
                ContentType = "application/json";
            }

            return new Response
            {
                StatusCode = (int)StatusCode,
                ContentType = ContentType,
                Content = contentBytes,
            };
        }

        public static ResponseJson NewResponseJson(uint retcode = 0, string? message = null, object? data = null)
        {
            ResponseJson rsp = new ResponseJson()
            {
                Data = data ?? string.Empty,
                Message = message ?? string.Empty,
                Retcode = retcode
            };
            return rsp;
        }
    }

    public static class Http
    {
        public static string? Post(HttpListenerRequest request)
        {
            string requestBody;
            var encoding = request.ContentEncoding ?? Encoding.UTF8; // 使用默认编码
            using (var reader = new StreamReader(request.InputStream, encoding))
            {
                requestBody = reader.ReadToEnd();
            }

            string contentType = request.ContentType ?? string.Empty; // 确保 contentType 不为 null

            switch (contentType)
            {
                case "application/json":
                    if (string.IsNullOrEmpty(requestBody))
                    {
                        // 返回错误响应或默认值
                        return null;
                    }
                    else
                    {
                        object? jsonData = JsonConvert.DeserializeObject(requestBody);

                        if (jsonData == null)
                        {
                            // 处理解析失败的情况
                            return null;
                        }

                        // 如果需要返回字符串形式，可以使用 jsonData.ToString()
                        return jsonData.ToString();
                    }
                default:
                    // 处理其他 ContentType，或者返回 null
                    return null;
            }
        }
    }

    public class Route
    {
        public string Path { get; set; }
        public string Method { get; set; }
        public Func<HttpListenerRequest, Response> Handler { get; set; }

        public Route()
        {
            // 初始化属性为非空的默认值
            Path = string.Empty;
            Method = "GET";
            Handler = DefaultHandler;
        }

        private Response DefaultHandler(HttpListenerRequest request)
        {
            // 默认处理函数，返回 404
            return new Response
            {
                StatusCode = 404,
                ContentType = "text/plain; charset=utf-8",
                Content = Encoding.UTF8.GetBytes("Not Found")
            };
        }

        public string HandlerName => Handler.Method.Name;
    }

    public interface IRouteProvider
    {
        Route[] GetRoutes();
        bool IsNeedInit { get; }
        void Init();
    }
}
