using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using LoggingLib;
using System.Collections.Generic;
using System;

namespace ProxyClient {
    public class EscalationService 
    {
        
        private readonly IDbModel _model;
        public EscalationService(IDbModel model){
            _model=model;

        }
        public async Task checkEscalation() 
        {
            List<Tuple<string,string>> notifications= _model.GetPendingNotifications();
            DateTime tempDate;
            foreach(Tuple<string, string> notification in notifications){
                tempDate=DateTimeHelpers.Parser(notification.Item1);//getting notification time & date;
                Console.WriteLine(tempDate.ToString());
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
                await Task.Delay(System.TimeSpan.FromMinutes(2), stoppingToken);
            }
        }
    }
}