using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WhatsappService
{
    static class  MainProgram
    {
        static void Main (string[] args)
        {     
            //public address of the webservice
            string PUBLIC_ADDRESS=Environment.GetEnvironmentVariable("PUBLIC_ADDRESS");     
            //temp configuration
            string TEMP=Environment.GetEnvironmentVariable("TEST_NUMBER");
            //building the webservice
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();
            app.Urls.Add("http://localhost:8080");//change to 8080
            app.Urls.Add("https://localhost:8000"); //change to 8000
            //instantiating the twilio client
            Client client = new Client();
            //test url endpoints
            app.MapGet( "/" , () => "Welcome to the main portal");
            app.MapGet("/checkGet" , checkGet);
            app.MapPost("/checkPost", checkPost);
            app.MapPost("/trialMessage", (clientResponse response)=>{
                Console.WriteLine(response);
                string picture_url=PUBLIC_ADDRESS+"/index.jpg";
                //Console.WriteLine("body of UserMessage: " + response.body);
                client.sendMessage(TEMP,"Reply to message", picture_url);
                //https://timberwolf-mastiff-9776.twil.io/demo-reply
            });
            //url_endpoint to send the notifications to user 
            app.MapGet("/notification", ( [FromQuery(Name = "name")]string name )=>
            {   
                string picture_url=PUBLIC_ADDRESS+"/index.jpg";
                client.sendMessage(TEMP, $"This is message of {name}", picture_url);
                Console.WriteLine(name);
                return new { id = 1 };
            });
            app.MapGet("/TrialNotification", ()=>
            {   
                string picture_url=PUBLIC_ADDRESS+"/index.jpg";
                client.sendMessage(TEMP, "This is test message", picture_url);
                return new { id = 1 };
            });
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
        static async Task<string> MessageStatus ( HttpContext context ) {
            string temp = await HandlingPostForm.toString(context);
            Status status = HandlingPostForm.StatusResponse(temp);
            Console.WriteLine(status);
            return "Checked out";
        }
        //Delegate for handling the message from the user
        [Consumes("application/x-www-form-urlencoded")] 
        static async Task<string> UserMessage ( HttpContext context ) {
            string temp = await HandlingPostForm.toString(context);
            UserMessageContainer response = HandlingPostForm.UserResponse(temp);
            Console.WriteLine(response.Body);
            return "Checked out";
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
    }

}


