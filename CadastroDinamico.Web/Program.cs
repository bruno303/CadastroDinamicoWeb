using CadastroDinamico.Web.Extension;
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
                .UseKestrel((hostingContext, opts) =>
                {
                    opts.Listen(IPAddress.Loopback, hostingContext.RetornarPortaAmbiente());
                })
                .UseStartup<Startup>();

        private static void ConfigurarAplicacao()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR", false);
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR", false);
        }
    }
}
