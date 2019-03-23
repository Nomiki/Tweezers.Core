using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Tweezers.Api.Controllers;
using Tweezers.Api.DataHolders;
using Tweezers.Discoveries.Containers;
using Tweezers.Discoveries.Engine;

namespace Tweezers.Api.Middleware
{
    public class TweezersMiddleware
    {
        public static void AddIgnoreNullService(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(options => {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });
        }
    }
}
