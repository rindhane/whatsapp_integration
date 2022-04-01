// See https://aka.ms/new-console-template for more information
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using LoggingLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using dbSetup;
using ProductionService;
using ProductionQueryLib;
using System.Collections.Generic;
using Newtonsoft.Json;
using proxyArrangement;

namespace ProxyClient {
    public class MainProgram {

        static void Main(string[] args) 
        {
            //building the webservice
            var builder = WebApplication.CreateBuilder(args);
            //builder.Configuration
            //capturing the confinguration for the same
            builder.Host.ConfigureAppConfiguration((hostingContext,config)=>{
                config.AddJsonFile("config.json", //make it config.json
                                    optional:true,
                                    reloadOnChange:true);
            });
            //Adding Services
            //instantiating the twilio client
            builder.Services.AddSingleton<ImessageClient,Client>();
            //instantiating the internal db record service
            builder.Services.AddTransient<IDbModel, DbConnection>();
            //instantiating the logger
            builder.Services.AddTransient<IlogWriter, logWriter>();
            //instantiating the ProductionDataServices
            builder.Services.AddTransient<ProductionFetcherClass>(sp=> new ProductionFetcherClass("dbData.txt"));
            builder.Services.AddHostedService<ProductionFetcherService>();           
            //getting the builder ready;
            //add background services 
            builder.Services.AddSingleton<EscalationService>();
            builder.Services.AddHostedService<EscalationBackgroundService>();
            //add proxy monitoring (arangement to keep the token alive)
            builder.Services.AddSingleton<ChromiumSession>();
            builder.Services.AddHostedService<ProxyActiveService>();
            //remove the backgroundservice halting the host due to unhandled exception : 
            builder.Services.Configure<HostOptions>(
                hostOptions=>
                {
                    hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
                }
            );
            //build the app
            var app = builder.Build();
            //extracting the working port
            string? httpPort = app.Services.CreateScope()
                            .ServiceProvider
                            .GetRequiredService<IConfiguration>()["Configs:PortDetails:Default"];
            app.Urls.Add($"http://0.0.0.0:{httpPort}");
            //test url endpoints
            app.MapGet( "/" , () => "Welcome to the main portal");
            app.MapPost("/trialMessage", trialMessage);
            app.MapPost("/notification", notification);
            app.MapPost("/productionData", getProductionData);
            app.MapGet("/testProductionData", testProductionData);
            app.MapPost("/UserMessage", ()=>"result achieved");
            app.MapPost("/MessageStatus", ()=>"status received");
            //to serve files
            app.UseStaticFiles();
            //start the worker service
            app.Run();
        }
        [Consumes("text/plain")]
        static async Task trialMessage (HttpContext context, 
                                        ImessageClient client,
                                        IConfiguration Configuration )
        {
            Console.WriteLine(Configuration["Configs:Proxy_Details:id"]);
            client.sendMessage( Configuration["TEST_NUMBER"],
                                "Trial message was sent", 
                                null);
            await context.Response.WriteAsync("trial succeeded");
        }
        [Consumes("text/plain")]
        static async Task notification(
                        HttpContext contxt, 
                        ImessageClient client,
                        IlogWriter logger,
                        IDbModel model
                        )
        {   
            string paramString= await HandlingPostForm.toString(contxt);
            string[] parameters=paramString.Split(';');
            logger.writeNotification($"notification> {paramString}");
            //extract IP from parameter[1] i.e device name& id for device-id //pending :future work
            //remove spaces from the string 
            string status = parameters[2].Replace(" ", String.Empty);
            string deviceID = parameters[1].Replace(" ", String.Empty);
            //get the action type from the status(parameters[2]) string
            int action=Classifiers.statusEncoders(status); 
            Console.WriteLine($"action:{action}");
            if (action!=0) //unmapped statuses to actions should not be sent to database
            {
                long escalationState=0;
                //store parameters into the db
                model.updateEscalation(deviceID, status, action,escalationState);
            }
            //send instant notification to the group A people
            string notification = Templates.notification_Message(
                parameters[0], parameters[1], parameters[2], parameters[3]);
            //only sending the notification to users in group-level A for instant notification
            string group="A"; //change to group A
            DialogFlow.sendNotificationMessage(notification,client, model, logger, group);
            await contxt.Response.WriteAsync("notification obtained");
            //return new { id = 1 };
        }
        //[Consumes("text/plain")]
        [Consumes("application/json")]
        static async Task getProductionData([FromBody] Dictionary<string,Dictionary<string, int>> result, 
                                            HttpContext contxt,
                                            ImessageClient client,
                                            IlogWriter logger,
                                            IDbModel model)
        {
            //StreamReader reader = new StreamReader(contxt.Request.Body);
            //string paramString= await reader.ReadToEndAsync();
            logger.writeNotification($"productionData> {JsonConvert.SerializeObject(result)}");
            //generate production Message for People to the group A people
            //all persons are within the group A
            string group="A";
            //currently there is no production segregation strategy write function for it.  
            //string notification = Templates.productionUpdate_message(result); //earlier template message which has been discarded 
            string notification = Templates.productionFormattedUpdate_message(result);
            DialogFlow.sendProductionUpdate(notification,client, model, logger, group);
            await contxt.Response.WriteAsync("production details Updated");
            //return new { id = 1 };
        }

    static async Task testProductionData( HttpContext contxt,
                                            ImessageClient client,
                                            IlogWriter logger,
                                            IDbModel model)
        {
            //StreamReader reader = new StreamReader(contxt.Request.Body);
            //string paramString= await reader.ReadToEndAsync();
            Dictionary<string,Dictionary<string, int>> result = new Dictionary<string, Dictionary<string, int>>();
            result.Add("Q Block", new Dictionary<string, int>{
                {"LTV",30},
                {"MPV",20}
            });
            result.Add("P Block", new Dictionary<string, int>{
                {"HCV",30},
                {"ICV",201},
                {"LCV",16}
            });
            result.Add("R Block", new Dictionary<string, int>{
                {"BLR",2},
                {"BLRPICKUP",12},
                {"BLR108",0}
            });
            result.Add("S Block", new Dictionary<string, int>{
                {"S101",32},
                {"W601",591}
            });
            result.Add("T Block", new Dictionary<string, int>{
                {"Z101",2},
                {"U301",22}
            });
            logger.writeNotification($"productionData> {JsonConvert.SerializeObject(result)}");
            //generate production Message for People to the group A people
            //all persons are within the group A
            string group="A";
            //currently there is no production segregation strategy write function for it.  
            string notification = Templates.productionFormattedUpdate_message(result);
            DialogFlow.sendProductionUpdate(notification,client, model, logger, group);
            await contxt.Response.WriteAsync("production details Updated");
            //return new { id = 1 };
        }
    }
}