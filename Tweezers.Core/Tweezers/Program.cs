using System.IO;
using Microsoft.AspNetCore.Hosting;
using Tweezers.MetadataManagement;

namespace Tweezers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TweezersMetadata.Init("tweezers-settings.json");
            CreateWebHostBuilder().Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder()
        {
            int port = TweezersMetadata.Instance.TweezersDetails.Port;
            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls($"https://localhost:{port}/");
        }
    }
}
