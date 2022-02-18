using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using LoggingLib;



namespace ProxyClient {
    public class EscalationService 
    {
        public async Task checkEscalation() 
        {
            int count = 0;
            while(true)
            {
                await Task.Delay(System.TimeSpan.FromMinutes(1));
                System.Console.WriteLine($"Service is running true with running count:{count}");
                count++;
            }
        }
    }


    public class EscalationBackgroundService : BackgroundService
    {
        private readonly EscalationService _watcherService;
        private readonly IlogWriter _logger;

        public EscalationBackgroundService(EscalationService EscalationService, IlogWriter logger)
        {
            (_watcherService,_logger) = (EscalationService,logger);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _watcherService.checkEscalation();
                //string response = await _watcherService.GetNotificationAsync();
                //_logger.writeNotification(response);
                //System.Console.WriteLine(response);
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //await Task.Delay(System.TimeSpan.FromMinutes(2), stoppingToken);
            }
        }
    }
}