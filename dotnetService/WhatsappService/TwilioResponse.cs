using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace WhatsappService{
    public record Status(
                string? To ,
                string? AccountSid,
                string? ApiVersion,
                string? ChannelPrefix,
                string? ChannelInstallSid,
                string? SmsSid,
                string? MessageStatus,
                string? From,
                string? MessageSid,
                string? ChannelToAddress,
                string? SmsStatus,
                string? StructuredMessage
            ) {};
    public record  subresourceUri (string media) {};
    public record clientResponse (
        string date_created,
        string date_updated,
        string date_sent,
        string account_sid,
        string to ,
        string from ,
        string messaging_service_sid,
        string body,
        string status,
        string num_segments,
        string num_media ,
        string direction,
        string api_version,
        string price,
        string price_unit,
        string error_code,
        string error_message,
        string uri,
        subresourceUri subresource_uris
        )  {};

     public class UserMessageContainer
    {
        public string SmsMessageSid { get; set; }
        public string SmsSid { get; set; }
        public string MessageSid {get;set;}
        public string AccountSid {get;set;}
        public string SmsStatus { get; set; }
        public int NumMedia { get; set; }
        public string ProfileName{ get; set; }
        public string WaId { get; set; } 
        public string Body {get; set;}
        public string ButtonText {get; set;}
        public string To {get; set;}
        public string From {get; set;}
        public int NumSegments {get; set;}
        public string ApiVersion {get; set;}
    }

    public static class HandlingPostForm 
    {
        
        //convert HttpContext of application/x-www-form-urlencoded to string object
        public static async Task<string> toString(HttpContext context)
        {
            using StreamReader reader = new StreamReader(context.Request.Body);
            //string temp =  await reader.ReadToEndAsync();
            //Console.WriteLine(temp);
            return await reader.ReadToEndAsync();
            
        }

        //Providing POCO Status Object of Status Response from twilio from body string
        public static Status StatusResponse(string contextString){
            return Helpers.ConversionHelpers.queryString2Status(contextString);

        }

        
        ////Providing POCO Status Object of User Response from twiliobody string
        public static UserMessageContainer UserResponse(string contextString)
        {
            return Helpers.ConversionHelpers.queryString2UserResponse(contextString);
        } 
    }

}

