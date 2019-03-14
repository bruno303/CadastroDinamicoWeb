using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;
using System.Net;

namespace CadastroDinamico.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigurarAplicacao();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(opts =>
                {
                    opts.Listen(IPAddress.Loopback, 5000);
                })
                .UseStartup<Startup>();

        private static void ConfigurarAplicacao()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR", false);
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR", false);
        }
    }
}
