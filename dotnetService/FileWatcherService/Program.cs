using FileWatcherService;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options=>{
        options.ServiceName= ".Net FileWatcher Service";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<WindowsBackgroundService>();
        //services.AddHttpClient<WatcherService>();
        services.AddHttpClient<WatcherService>();
    })
    .Build();

await host.RunAsync();
