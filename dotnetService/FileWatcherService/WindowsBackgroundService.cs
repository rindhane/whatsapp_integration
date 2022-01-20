namespace FileWatcherService;

public class WindowsBackgroundService : BackgroundService
{
    private readonly WatcherService _watcherService;
    private readonly ILogger<WindowsBackgroundService> _logger;

    public WindowsBackgroundService(WatcherService watcherService,
                                    ILogger<WindowsBackgroundService> logger)
    {
        (_watcherService,_logger) = (watcherService,logger);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            string response = await _watcherService.GetNotificationAsync();
            _logger.LogWarning(response);
            //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
        }
    }
}
