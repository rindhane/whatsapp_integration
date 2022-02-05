using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using dbSetup;
using Microsoft.Extensions.DependencyInjection;
using LoggingLib;
using System.IO;
using ProductionService;

namespace WhatsappService
{
    static class  MainProgram
    {
        static void Main (string[] args)
        {     
            //building the webservice
            var builder = WebApplication.CreateBuilder(args);
            //Adding Services
            //instantiating the sessionDb
            builder.Services.AddTransient<IDbModel, DbConnection>();
            //instantiating the twilio client
            builder.Services.AddSingleton<ImessageClient,Client>();
            //instantiating the host details
            builder.Services.AddSingleton<IHostDetails,HostDetails>();
            //instantiating the logger
            builder.Services.AddTransient<IlogWriter, logWriter>();
            //instantiating the ProductionData connector
            builder.Services.AddTransient<IProductionFetcher, ProductionFetcher>();
            //builder.Configuration
            var app = builder.Build();
            string? httpPort = Environment.GetEnvironmentVariable("httpPort");
            string? httpsPort=Environment.GetEnvironmentVariable("httpsPort");
            app.Urls.Add($"http://0.0.0.0:{httpPort}");
            //app.Urls.Add($"https://0.0.0.0:{httpsPort}"); 
            //test url endpoints
            app.MapGet( "/" , () => "Welcome to the main portal");
            app.MapGet("/checkGet" , checkGet);
            app.MapPost("/checkPost", checkPost);
            app.MapPost("/trialMessage", trialMessage);
            //url_endpoint to send the notifications to user 
            app.MapPost("/notification", notification);
             //url_endpoint to register the new User
            app.MapGet("/NewUserRegistration/{id}", userRegistration);
            //url_endpoint to receive the user responses 
            app.MapPost("/UserMessage", UserMessage);
            //url_endpoint to receive the message status
            app.MapPost("/MessageStatus", MessageStatus );
            //enabled the serving staticfiles for twilio service to fetch image/media files 
            app.UseStaticFiles();
            //start the worker service
            app.Run();        
        }
        //Delegate for handling the message-status from the Twilio service
        [Consumes("application/x-www-form-urlencoded")] 
        static async Task<string> MessageStatus ( HttpContext context, IDbModel model ) {
            string temp = await HandlingPostForm.toString(context);
            Status status = HandlingPostForm.StatusResponse(temp);
            model.updateStatus(status);
            //Console.WriteLine(status);
            return "Checked out";
        }
        //Delegate for handling the message from the user
        [Consumes("application/x-www-form-urlencoded")] 
        static async Task UserMessage ( HttpContext context, ImessageClient client, IDbModel model,IProductionFetcher productionDb, IHostDetails host, IWebHostEnvironment hostEnvironment ) {
            string temp = await HandlingPostForm.toString(context);
            UserMessageContainer response = HandlingPostForm.UserResponse(temp);
            //Console.WriteLine(response.Body+ $": {response.SmsMessageSid} : {response.ButtonText}");
            host.appRoot=hostEnvironment.ContentRootPath;
            host.webRoot=hostEnvironment.WebRootPath;
            await DialogFlow.ConversationHandler(response, client, model, host ,productionDb);
            //return "Got it";
        }
        static async Task checkGet (HttpContext contxt)
        {
            string str="hello task has run";
            Console.WriteLine(str);
            await contxt.Response.WriteAsync(str);
        }

       [Consumes("application/x-www-form-urlencoded")] 
        static async Task<string> checkPost ( HttpContext context ) {
            Console.WriteLine(await HandlingPostForm.toString(context));
            return "Checked out";
        }
        static async Task trialMessage (HttpContext context, clientResponse response, IHostDetails host, ImessageClient client)
        {
            Console.WriteLine(response);
            string picture_url=host.PUBLIC_ADDRESS+"/index.jpg";
            //Console.WriteLine("body of UserMessage: " + response.body);
            client.sendMessage(host.TEMP,"Reply to message", picture_url);
            //https://timberwolf-mastiff-9776.twil.io/demo-reply
            await context.Response.WriteAsync("trial succeeded");
        }
        [Consumes("text/plain")]
        static async Task notification(
                        HttpContext contxt,
                        IHostDetails host, 
                        ImessageClient client,
                        IlogWriter logger,
                        IDbModel model )
        {   
            StreamReader reader = new StreamReader(contxt.Request.Body);
            string paramString= await reader.ReadToEndAsync();
            string[] parameters=paramString.Split(';');
            logger.writeNotification($"notification> {paramString}");
            string notification = Templates.notification_Message(
                parameters[0], parameters[1], parameters[2], parameters[3]);
            //only sending the notification to users in  group A
            string group="Y"; //change to group A
            DialogFlow.sendNotificationMessage(notification,client, model, logger, group);
            await contxt.Response.WriteAsync("notification obtained");
            //return new { id = 1 };
        }
        static async Task <object> userRegistration(int id, IHostDetails host, ImessageClient client)
            {   
                //string picture_url=PUBLIC_ADDRESS+"/index.jpg";
                client.sendMessage(host.TEMP,Templates.Greeting_Message(), null);
                return new { id = 1 };
            }
    }

}


