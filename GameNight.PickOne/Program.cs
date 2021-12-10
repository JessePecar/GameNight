using GameNight.API;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

public class Program
{
    
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webhost => 
        webhost
        .UseStartup<Startup>()
        .ConfigureKestrel(options =>
        {
            options.Limits.MinRequestBodyDataRate =
                         new MinDataRate(bytesPerSecond: 100,
                             gracePeriod: TimeSpan.FromSeconds(10));
            options.Limits.MinResponseDataRate =
                new MinDataRate(bytesPerSecond: 100,
                    gracePeriod: TimeSpan.FromSeconds(10));
            options.Listen(IPAddress.Loopback, 9191);
            options.Limits.KeepAliveTimeout = TimeSpan.FromHours(5);
        }));
}