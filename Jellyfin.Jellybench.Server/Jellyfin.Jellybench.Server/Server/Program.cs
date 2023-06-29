using System.Threading.RateLimiting;
using Jellyfin.Jellybench.Database;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ServiceLocator.Discovery.Option;
using ServiceLocator.Discovery.Service;

namespace Jellyfin.Jellybench.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();
        builder.Services
            .AddDbContext<JellybenchDataContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Jellybench")))
            .AddDbContextFactory<JellybenchDataContext>();

        builder.Services.UseServiceDiscovery()
            .FromAssembly(typeof(Program).Assembly)
            .DiscoverOptions(builder.Configuration)
            .FromAssembly(typeof(Program).Assembly)
            .LocateServices();

        builder.Services.AddRateLimiter(_ =>
        {
            _.AddFixedWindowLimiter("UploadLimiter", op =>
            {
                op.PermitLimit = 1;
                op.Window = TimeSpan.FromHours(1);
                op.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                op.QueueLimit = 2;
            });
        });

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Jellybench", Version = "v1" });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();


        app.MapRazorPages();
        
        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}