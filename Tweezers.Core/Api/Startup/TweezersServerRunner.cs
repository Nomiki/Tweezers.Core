using System.IO;
using Microsoft.AspNetCore.Hosting;
using Tweezers.Api.MetadataManagement;

namespace Tweezers.Api
{
    public static class TweezersServerRunner
    {
        public static void Start(string[] args)
        {
            TweezersMetadata.Init("tweezers-settings.json");
            CreateWebHostBuilder().Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder()
        {
            int port = TweezersMetadata.Instance.TweezersDetails.Port;
            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls($"https://0.0.0.0:{port}/");
        }
    }
}
