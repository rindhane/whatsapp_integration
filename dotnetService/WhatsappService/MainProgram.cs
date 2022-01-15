using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace WhatsappService
{
    class MainProgram
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
            app.Urls.Add("http://localhost:8080");
            app.Urls.Add("https://localhost:8000");
            //instantiating the twilio client
            Client client = new Client();
            //test url endpoints
            app.MapGet( "/" , () => "Hello World!");
            app.MapGet("/work" ,()=>new { id = 1 });
            app.MapPost("/check" , (string statement) => {
                Console.WriteLine("statement",statement);
                return new { id = 1 };
            });
            //url_endpoint to send the notifications to user 
            app.MapGet("/notification",()=>
            {   
                string picture_url=PUBLIC_ADDRESS+"/index.jpg";
                client.sendMessage(TEMP, "This is test message", picture_url);
                return new { id = 1 };
            });
             //url_endpoint to receive the user responses 
            app.MapPost("/userMessage",(statusResponse response)=>{
                string picture_url=PUBLIC_ADDRESS+"/index.jpg";
                //Console.WriteLine("body of UserMessage: " + response.body);
                client.sendMessage(TEMP,"Reply to message", picture_url);
                //https://timberwolf-mastiff-9776.twil.io/demo-reply
            });
            //url_endpoint to receive the message status
            app.MapPost("/status", ( [FromBody] string response)=> {
                Console.WriteLine(response);
                return new { id = 1 };
            });
            //enabled the serving staticfiles for twilio service to fetch image/media files 
            app.UseStaticFiles();
            //start the worker service
            app.Run();        
        }
    }

}


