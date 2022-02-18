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

namespace ProxyClient {
    public class MainProgram {

        static void Main(string[] args) 
        {
            //building the webservice
            var builder = WebApplication.CreateBuilder(args);
            //builder.Configuration
            //capturing the confinguration for the same
            builder.Host.ConfigureAppConfiguration((hostingContext,config)=>{
                config.AddJsonFile("config_temp.json", //make it config.json
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
            //getting the builder ready;
            //add background services 
            builder.Services.AddSingleton<EscalationService>();
            builder.Services.AddHostedService<EscalationBackgroundService>();
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
                //store parameters into the db
                model.updateEscalation(deviceID, status, action);
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
    }
}