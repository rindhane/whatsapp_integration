using System;
using Twilio;
using System.Linq;
using Twilio.Rest.Api.V2010.Account;

namespace WhatsappService
{
    //wrapper class to work with the twilio service
    public class Client {

        private string accountNumber=Environment.GetEnvironmentVariable("ACCOUNT_NUMBER");
        //constructor purpose is to instantiate the twilio api client
        public Client ()
        {
            string? accountSid= Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            string? authToken= Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN"); 
            TwilioClient.Init(accountSid,authToken);
        }
        //method to transfer the message to the twilio service
        public void sendMessage (string phone, string newMessage, string url)
        {
            var messageOptions = new CreateMessageOptions(
                                        new Twilio.Types.PhoneNumber($"whatsapp:{phone}")
                                        );
            messageOptions.From = new Twilio.Types.PhoneNumber( $"whatsapp:{accountNumber}" );
            messageOptions.Body = newMessage ;
            //Console.WriteLine("url: " + url);
            var mediaUrl = new [] {
                                    new Uri(url)
                                            }.ToList();
            messageOptions.MediaUrl = mediaUrl;
            var message = MessageResource.Create(messageOptions); 
            Console.WriteLine("messageBody : " + message.Body);
            Console.WriteLine("messageID : " + message.Sid);
        }
    }

    
}