//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace FileWatcherService
{
    class MainProgram
    { 
        
        public static async Task Main (string[] args)
        {
        
            string DudeFolderPath= args[0]; //@"..\testSample\";
            string NotificationUrl=args[1];//"http://localhost:8000/notification";
            string machineName=args[2];//"dude-critical";
            httpSender httpClient= new httpSender();
            WatcherService watcher = new WatcherService(httpClient,NotificationUrl);
            watcher.DudeFolderPath=DudeFolderPath;
            watcher.NotificationUrl=NotificationUrl;
            watcher.ServiceName=machineName;
            string LogDate;
            while (true) 
            {
                LogDate=System.DateTime.Now.ToShortDateString();
                string filePath= watcher.dudeFilePicker();
                if (filePath!=null)
                {
                    System.Console.WriteLine($"Reading the {filePath} on {System.DateTime.Now.ToString()}");
                    await watcher.GetNotificationAsync(filePath,LogDate);
                }else {System.Console.WriteLine("No Log file was found");}
                await Task.Delay(System.TimeSpan.FromMinutes(2)); //wait time of two mins if nothing is working
            }

        /*
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
        */

        }
    }
}