using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net;

namespace CadastroDinamico.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
                //.UseUrls("http://*:3000");
                //.UseWebRoot(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
                //.UseContentRoot(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
    }
}
