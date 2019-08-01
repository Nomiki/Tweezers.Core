using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Tweezers.Api.DataHolders;

namespace Tweezers.Api.Middleware
{
    public class JsonExceptionMiddleware
    {
        private static readonly int ExceptionStacktraceDepth;

        static JsonExceptionMiddleware()
        {
            ExceptionStacktraceDepth = 10;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

            Exception exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (exception == null)
                return;

            TweezersErrorBody error = new TweezersErrorBody
            {
                Message = exception.Message,
                StackTrace = FetchStackTrace(exception),
                Method = exception.TargetSite.Name,
                Code = context.Response.StatusCode
            };

            context.Response.ContentType = "application/json";
            using (StreamWriter writer = new StreamWriter(context.Response.Body))
            {
                new JsonSerializer().Serialize(writer, error);

                await writer.FlushAsync().ConfigureAwait(false);
            }
        }

        private static string[] FetchStackTrace(Exception exception)
        {
            return exception.StackTrace.Split(Environment.NewLine).Take(ExceptionStacktraceDepth).Select(l => l.Trim())
                .ToArray();
        }
    }
}