using CadastroDinamico.Web.Extension;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Net;

namespace CadastroDinamico.Web
{
    public class Program
    {
        private static IConfigurationRoot configurationRoot;

        public static void Main(string[] args)
        {
            ConfigurarAplicacao();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IConfigurationRoot CreateConfiguration(string path) =>
            new ConfigurationBuilder().AddJsonFile(path, false, true).Build();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            int httpPort = GetValue<int>("httpPort");
            int httpsPort = GetValue<int>("httpsPort");

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureKestrel(opts =>
                {
                    opts.Listen(IPAddress.Loopback, httpPort);
                    opts.Listen(IPAddress.Loopback, httpsPort, config =>
                    {
                        config.UseHttps();
                    });
                })
                .UseStartup<Startup>();
        }

        private static void ConfigurarAplicacao()
        {
            string environmentFileName = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";
            configurationRoot = CreateConfiguration(System.IO.File.Exists(environmentFileName) ? environmentFileName : "appsettings.json");

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(GetValue<string>("cultureInfo"), false);
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(GetValue<string>("cultureInfo"), false);
        }

        private static TType GetValue<TType>(string key) => configurationRoot.GetValue<TType>($"hosting:{key}");
    }
}
