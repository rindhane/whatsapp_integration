using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IO; 
using dbSetup;
using System.Collections.Generic;
using LoggingLib;
using System;
using System.Globalization;

namespace ProxyClient {
    public static class Templates {
        public static string notification_Message(string probe, string device, string status, string description) {
            return
            $"Critical Alert Notification:\nService {probe} on {device} is now {status} with the following issue {description}";
        }
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
    }

    public interface ImessageClient {
        public MessageRecord sendMessage(string phone, string newMessage, string url); //implements the message-client-api for sending the message
    }

    public class MessageRecord{
        public string MessageID {get;set;}
        public string DialogueID {get;set;}
        public string TIME_RECORD {get;set;}
        public string KIND {get;set;}
        public string MessageText {get;set;}
        public string ReadStatus {get;set;}

        public override string ToString()
        {
            return base.ToString()+"||"+"messageID:"+ MessageID 
                                    + '|'+"DialogueID:" + DialogueID
                                    + '|'+ "TIME_RECORD:" + TIME_RECORD 
                                    + '|'+ "KIND:"+ KIND
                                    + '|'+ "MessageText:"+ MessageText
                                    + '|'+ "ReadStatus:" + ReadStatus;
        }
    }
    
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

    public interface IDbModel {
        public bool IsUser(string userPhone) ;

        public object[] GetUserIdRegistration(int userID);
        public object[] GetRegistrationDetails (string userPhone);
        public void DeleteRegistration(long userID);
        public object[] GetUserDetails(string userPhone);
        public object[] GetUserDetails(long userID);
        public void AddUser(object[] userDetails);
        public List<Tuple<long,string>> GetUserInProductionGroup(string group );

        public object[] GetSession(long userId);
        public void generateSession(MessageRecord record, long userId, int category, int stage);

        public void updateSession (MessageRecord record, object[] parentSession, int nextStage );
        public void clearSession (string parentSessionId);
        public void messageSent(MessageRecord record);
        public void messageReceived(UserMessageContainer response, string DialogueID);    
        public void updateStatus(Status status);
        public void updateEscalationStatus(string deviceID, string status) ;
        public void insertEscalation(string deviceID, string status);
        public void deleteEscalation(string deviceID);
        public void updateEscalation(string deviceID, string status, int type);
        public List<Tuple<long,string>> GetUserAlertGroup(string group );
        public List<Tuple<string,string>>GetPendingNotifications();

    }

    public class Classifiers {
        public static int statusEncoders(string status)
        {
            Dictionary<string,int> statusDict = new Dictionary<string,int>();
            statusDict.Add("down",1); //bad things to add for monitoring
            statusDict.Add("up",-1); //ok things to remove from the monitoring
            int intStatus=0;//out 0 for unkown statuses
            statusDict.TryGetValue(status.ToLower(), out intStatus);
            return intStatus;
        } 
    }
    public class DialogFlow {

        public static void sendNotificationMessage(
            string Notification,
            ImessageClient client, 
            IDbModel model, 
             IlogWriter logger, 
             string group)
        {
            //get user in the group
            List<Tuple<long,string>> users = model.GetUserAlertGroup(group);
            foreach(Tuple<long,string> userDetails in users)
            {
                //sending the notifications
                MessageRecord message=client.sendMessage(userDetails.Item2 , Notification , null);
                logger.writeNotification($"Message sent {message.MessageID} to user: {userDetails.Item2}");
                //session management
                //message.DialogueID=GenerateDialogId().ToString();
                //message.KIND="Outgoing";
                //model.messageSent(message);
                //object[] session = GetSession(userDetails.Item1 ,model);
                //clearing existing session
                //if (session!=null)
                //{
                //    model.clearSession((string)session[0]); //based on SessionID column index in sessionstatus in record database
                //}
                //this session is part of category 1
                //int cat =1;
                //int stage =1;
                //model.generateSession(message, userDetails.Item1,cat, stage);
            } 
            Console.WriteLine("it runs");// pending
            return ;
        } 
        public static Guid GenerateSessionId(){
            return Guid.NewGuid();
        }
    
    } 

    public class DateTimeHelpers
    {
        public static string dateStringFormat(){
            string formatString= "dd-MM-yyyy HH:mm:ss";
            return formatString;
        }

        public static DateTime Parser(string s){
            CultureInfo enIN = CultureInfo.CreateSpecificCulture("en-IN");
            DateTimeFormatInfo dtfi = enIN.DateTimeFormat;
            dtfi.ShortDatePattern=dateStringFormat();
            return DateTime.ParseExact(s,dateStringFormat(),enIN);
        }
        
    }
}