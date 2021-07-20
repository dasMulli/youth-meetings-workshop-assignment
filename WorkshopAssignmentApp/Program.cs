using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Services;
using WorkshopAssignmentApp.Data;
using Microsoft.EntityFrameworkCore;
using WorkshopAssignmentApp.Assignment;

namespace WorkshopAssignmentApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services
                .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
                .AddMudServices()
                .AddDbContext<WorkshopAssignmentDbContext>(
                    options => options.UseInMemoryDatabase("WorkshopAssignment"),
                    ServiceLifetime.Transient
                )
                .AddTransient<AssignmentGenerator>();

            await builder.Build().RunAsync();
        }
    }
}
