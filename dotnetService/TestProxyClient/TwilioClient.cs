using System;
using Twilio;
using System.Linq;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net;
using System.Text;
using Twilio.Clients;

namespace ProxyClient
{
    //wrapper class to work with the twilio service
    public class Client:ImessageClient {

        private HttpClient _httpClient;
        private TwilioRestClient twilioRestClient;
        private string accountNumber=Environment.GetEnvironmentVariable("ACCOUNT_NUMBER");
        //constructor purpose is to instantiate the twilio api client
        public Client (IConfiguration Configuration)
        {
            //client authentication creds
            string? accountSid=Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            string? authToken=Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN"); 
            //get proxy details:
            string? proxyUrl=Configuration["Configs:Proxy_Details:proxyUrl"];
            string? ProxyUsername=Configuration["Configs:Proxy_Details:username"];
            string? ProxyPassword=Configuration["Configs:Proxy_Details:password"];
            bool useProxy = String.Equals(Configuration["Configs:Proxy_Details:useProxy"],"true") ? true : false;
            Console.WriteLine(useProxy);
            Console.WriteLine($"following proxyID configured {Configuration["Configs:Proxy_Details:id"]}");
            //TwilioClient.Init(accountSid,authToken);
            //proxied Client creation
                //creating custom proxy client :
                if (_httpClient == null)
                {
                    // It's best* to create a single HttpClient and reuse it
                    // * See: https://goo.gl/FShAAe
                    CreateHttpClient(proxyUrl,ProxyUsername, ProxyPassword, useProxy);
                }           
            twilioRestClient = new TwilioRestClient(
                accountSid,
                authToken,
                httpClient: new Twilio.Http.SystemNetHttpClient(_httpClient)
            );
            
        }
        //method to transfer the message to the twilio service
        public MessageRecord sendMessage (string phone, string newMessage, string url)
        {
            var messageOptions = new CreateMessageOptions(
                                        new Twilio.Types.PhoneNumber($"whatsapp:{phone}")
                                        );
            messageOptions.From = new Twilio.Types.PhoneNumber( $"whatsapp:{accountNumber}" );
            messageOptions.Body = newMessage ;
            //messageOptions.Client=twilioRestClient; //not available
            //Console.WriteLine("url: " + url);
            if (url!=null) { 
            var mediaUrl = new [] {
                                    new Uri(url)
                                            }.ToList();
            messageOptions.MediaUrl = mediaUrl;
            }
            var message = MessageResource.Create(messageOptions, client:twilioRestClient); 
            //Console.WriteLine("messageBody : " + message.Body);
            //Console.WriteLine("messageID : " + message.Sid);
            MessageRecord messageSent=new MessageRecord();
            messageSent.MessageID=message.Sid;
            messageSent.ReadStatus = message.Status.ToString();
            messageSent.TIME_RECORD=message.DateCreated.ToString();
            messageSent.MessageText=message.Body;
            //DateTime obj = DateTime.ParseExact(messageSent.TIME_RECORD,
            //                    "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
            //Console.WriteLine($"datetime1:{obj.ToString()}");
            //Console.WriteLine(messageSent.ToString());
            return messageSent;
        }
        
        private void CreateHttpClient(string ProxyUrl ,
                                             string ProxyUsername,
                                             string ProxyPassword,
                                             bool useProxy )
        {
            var proxyUrl = ProxyUrl;
            var handler = new HttpClientHandler()
            {
                Proxy = new WebProxy(proxyUrl),
                UseProxy = useProxy
            };

            _httpClient = new HttpClient(handler);
            var byteArray = Encoding.Unicode.GetBytes(
                ProxyUsername + ":" +
                ProxyPassword
            );

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(byteArray));
        }
    }

    
}