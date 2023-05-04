using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.PL.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!_logger.IsEnabled(LogLevel.Trace))
            {
                await _next.Invoke(context);
                return;
            }

            string requestLog = "{IPAddress} | Request Method: {HttpMethod}, Path: {HttpPath}";

            string bodyAsText;
            using (var bodyReader = new StreamReader(context.Request.Body))
            {
                bodyAsText = await bodyReader.ReadToEndAsync();
                if (!string.IsNullOrWhiteSpace(bodyAsText))
                {
                    string bodyAsTextFormatted = bodyAsText
                        .Replace("{", "{{")
                        .Replace("}", "}}");

                    requestLog += $"Body: {bodyAsTextFormatted}";
                }
            }

            _logger.LogTrace(requestLog, context.Connection.RemoteIpAddress, context.Request.Method, context.Request.Path);

            using (var injectedRequestStream = new MemoryStream())
            {
                var bytesToWrite = Encoding.UTF8.GetBytes(bodyAsText);

                await injectedRequestStream.WriteAsync(bytesToWrite, 0, bytesToWrite.Length);
                injectedRequestStream.Seek(0, SeekOrigin.Begin);
                context.Request.Body = injectedRequestStream;

                var sw = Stopwatch.StartNew();

                await _next.Invoke(context);

                //_logger.LogCritical("HTTP CONTEXT BODY LOG {HttpContext", Bo)
                _logger.LogTrace("Request execution took {RequestExecutionTime}", sw.Elapsed.TotalSeconds);
            }
        }
    }
}
