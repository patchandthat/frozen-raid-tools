using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FrozenGold.Web.Services;

namespace FrozenGold.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            var services = builder.Services;

            services.AddTransient(sp => new HttpClient
            {
                BaseAddress = new Uri("https://frozengoldapi.azurewebsites.net")
            });
            services.AddSingleton<GoldReportService>();

            await builder.Build().RunAsync();
        }
    }
}
