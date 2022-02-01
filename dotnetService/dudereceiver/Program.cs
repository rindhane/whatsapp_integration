using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using LoggingLib;
using System.Collections;

namespace dudenotification {
    public class data
{
    public string probe{ get; set; }
    public string device { get; set; } 
    public string description { get; set; } 
    public string status { get; set; }
    public string name { get; set; } 

}

public class notificationReceiver {
    static int Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        string? DUDE_PORT=Environment.GetEnvironmentVariable("DUDE_PORT");
        builder.Services.AddTransient<IlogWriter, logWriter>();
        var app = builder.Build();
        app.MapGet( "/" , () => "Welcome to the main portal");
        app.MapGet("/notification", checkGet);
        app.MapPost("/notification", checkPost);
        app.Urls.Add($"http://0.0.0.0:{DUDE_PORT}");
        //app.Urls.Add("https://0.0.0.0:9000");

        app.Run();

        static async Task checkGet (HttpContext contxt,
                                    [FromQuery(Name = "probe")] string Probe,
                                    [FromQuery(Name = "device")] string Device,
                                    [FromQuery(Name = "status")] string Status,
                                    [FromQuery(Name = "description")] string Description,
                                    [FromQuery(Name = "name")] string name )
        {
            //"Service [Probe.Name] on [Device.Name] is now [Service.Status] ([Service.ProblemDescription])";
            string str=$"Service {Probe} on {Device} is now {Status} ({Description})";
            Console.WriteLine($"name:{name}");
            await contxt.Response.WriteAsync(str);
            //return new { id = 1 };
        }
        //handling text/plain
        //https://stackoverflow.com/questions/58911465/unsupported-media-type-when-consuming-text-plain
        //[Consumes("text/plain")]
        [Consumes("text/plain")]
        static async Task checkPost (HttpContext contxt, 
                                        IlogWriter logger)
        {
            StreamReader reader = new StreamReader(contxt.Request.Body);
            string paramString= await reader.ReadToEndAsync();
            string[] parameters=paramString.Split(';');
            //"Service [Probe.Name] on [Device.Name] is now [Service.Status] ([Service.ProblemDescription])"; 
            string str=$"Service {parameters[0]} on {parameters[1]} is now {parameters[2]} ({parameters[3]}) from {parameters[4]}";
            logger.writeNotification(str);
            await contxt.Response.WriteAsync("result achieved");
            //return new { id = 1 };
        }
        return 0;
    }
}
}