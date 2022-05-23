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
        private readonly ImessageClient _client; 
        private readonly IlogWriter _logger;
        private Dictionary<long,double> EscalationTimeDict = 
        new Dictionary<long, double>()
        {
            {0, 0.33},//first escalation slab (alert) time is of 1/3 hrs=20mins
            {1, 0.9} //second escalation slab (critical) time is of 55 mins = 0.9 hrs

        };
        public EscalationService(IDbModel model, ImessageClient client, IlogWriter logger){
            _model=model;
            _client=client;
            _logger=logger;

        }

        public double getEscalationTimeThreshold (long escalationState){
            double defaultValue= escalationState; // any escalation above level 2 will be considered as infinitely expanding time slab
            double ans =  EscalationTimeDict.TryGetValue(escalationState, out double value) ? value * 60 * 60 :  defaultValue *60*60 ;
            return  ans ;//ans is returned in seconds.

        }

        public string getEscalationGroup (long escalationState){
            if (escalationState==0) {
                return "B";
            }
            if (escalationState==1){
                return "C";
            }
            if (escalationState>=2){
                return "C";
            }
            return null;
        }
        public async Task checkEscalation() 
        {
            //List<Tuple<string,string>> notifications= _model.GetPendingNotifications();
            List<object[]> notifications= _model.GetPendingNotifications();
            DateTimeOffset tempDate;
            foreach(object[] notification in notifications)
            {
                tempDate=DateTimeHelpers.Parser((string) notification[1]);//getting notification's timestamp;
                //unix time diff gap for int comparison;
                double diff = DateTimeOffset.Now.ToUnixTimeSeconds() - tempDate.ToUnixTimeSeconds();
                long escalationState = (long) notification[3];
                Console.WriteLine($"AtE:{escalationState}, d:{diff} & {getEscalationTimeThreshold(escalationState)}");
                if (
                    diff.CompareTo(
                        getEscalationTimeThreshold(escalationState)
                        ) > 0
                    )//check whether unix time is greather than threshold;
                {
                    //if larger then send the notification;
                    try {
                    string message = Templates.escalation_Message(
                        (string)notification[0],
                        TimeSpan.FromSeconds(diff).ToString()//received (string)notification[1] timestamp
                        );
                    //only sending the notification to users in specific group level for the notification
                    string group=getEscalationGroup(escalationState);
                    //stopping the notifications for testing 
                        //make the below statement into the comment to prvent it to send the notification
                    DialogFlow.sendNotificationMessage(message,_client, _model, _logger, group);
                        //uncomment the below statement for logging the message instead of sending it : 
                        //_logger.writeNotification($"NotSentMessage:{message}");
                    escalationState++;// increment the escalation state ;
                    //write the new increment into the database.
                    int action=Classifiers.statusEncoders((string)notification[2]);
                    _model.updateEscalation(
                        (string) notification[0],
                        (string) notification[2],
                        action,
                        escalationState);
                    }catch(Exception e){
                        _logger.writeNotification($"EscalationError: {e.Message} with state:{escalationState} of Message:{(string)notification[0]}");
                    }
                } 
                //do nothing if the date within escalation slab
                //Console.WriteLine("Nothing happened");
            }
            return ; // demarcate that none notification was found out; 
            //Console.WriteLine("no escalation notification was found out");
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