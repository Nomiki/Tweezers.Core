using System.IO;
using Microsoft.AspNetCore.Hosting;
using Tweezers.Api.MetadataManagement;

namespace Tweezers.Api.Startup
{
    public static class TweezersServerRunner
    {
        public static void Start(string[] args)
        {
            if (TweezersRuntimeSettings.Instance.IsInitialized)
            {
                TweezersInternalSettings.Init();
            }

            CreateWebHostBuilder().Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder()
        {
            int port = TweezersRuntimeSettings.Instance.Port;
            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls($"https://0.0.0.0:{port}/");
        }
    }
}
