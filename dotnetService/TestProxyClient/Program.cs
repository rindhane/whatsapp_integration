// See https://aka.ms/new-console-template for more information
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using LoggingLib;
using Microsoft.Extensions.Configuration;
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
                config.AddJsonFile("config.json", //change config.json
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
            string notification = Templates.notification_Message(
                parameters[0], parameters[1], parameters[2], parameters[3]);
            //only sending the notification to users in  group A
            string group="A"; //change to group A
            DialogFlow.sendNotificationMessage(notification,client, model, logger, group);
            await contxt.Response.WriteAsync("notification obtained");
            //return new { id = 1 };
        }
    }
}