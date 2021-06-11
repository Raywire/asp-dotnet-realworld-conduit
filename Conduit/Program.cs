using System.IO;
using Conduit.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Conduit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var root = "../";
            var dotenv = Path.Combine(root, ".env");
            DotEnv.Load(dotenv);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseUrls("http://*:80");
                });
    }
}
