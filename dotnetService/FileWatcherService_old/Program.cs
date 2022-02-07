using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using FileWatcherService;
using System.Threading.Tasks;

namespace FileWatcherService
{
    class MainProgram
    { 
        public static async Task Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .UseWindowsService(options=>{
                    options.ServiceName= ".Net FileWatcher Service";
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<WindowsBackgroundService>();
                    //services.AddHttpClient<WatcherService>();
                    services.AddSingleton<WatcherService>();
                    services.AddTransient<httpSender>();
                })
                .Build();
                await host.RunAsync();
        }
    }
}