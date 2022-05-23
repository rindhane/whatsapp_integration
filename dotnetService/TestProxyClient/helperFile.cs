using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IO; 
using dbSetup;
using System.Collections.Generic;
using LoggingLib;
using System;
using System.Globalization;
using System.Linq;

namespace ProxyClient {
    public static class Templates {
        public static string notification_Message(string probe, string device, string status, string description) {
            return
            $"Critical Alert Notification:\nService {probe} on {device} is now {status} with the following issue {description}";
        }

        public static string escalation_Message(string device, string time){
            return 
            $"Alert Notification:\n{device} has been down for following time {time}";
        }
        public static string productionUpdate_message(Dictionary<string,Dictionary<string, int>> result){
            Dictionary<string, int> QBlock = result["Q Block"];
            int LTV=0 ;
            int MPV=0;
            QBlock.TryGetValue("LTV",out LTV);
            QBlock.TryGetValue("MPV",out MPV);
            Dictionary<string, int> PBlock = result["P Block"];
            int HCV=0 ;
            int ICV=0;
            int LCV=0;
            PBlock.TryGetValue("HCV",out HCV);
            PBlock.TryGetValue("ICV",out ICV);
            PBlock.TryGetValue("LCV",out LCV);
            Dictionary<string, int> RBlock = result["R Block"];
            int BLR=0 ;
            int BLRPICKUP=0;
            int BLR108=0;
            RBlock.TryGetValue("BLR",out BLR);
            RBlock.TryGetValue("BLRPICKUP",out BLRPICKUP);
            RBlock.TryGetValue("BLR108",out BLR108);
            Dictionary<string, int> SBlock = result["S Block"];
            int S101=0 ;
            int W601=0;
            SBlock.TryGetValue("S101",out S101);
            SBlock.TryGetValue("W601",out W601);
            Dictionary<string, int> TBlock = result["T Block"];
            int Z101=0 ;
            int U301=0;
            TBlock.TryGetValue("Z101",out Z101);
            TBlock.TryGetValue("U301",out U301);
            return 
            $"Production Update:\nQ Block >> LTV:{LTV} |MPV:{MPV} \nP Block >> HCV:{HCV} |ICV:{ICV} |LCV:{LCV} \nR Block >> BLR:{BLR} |BLRPICKUP:{BLRPICKUP} |BLR108:{BLR108} \nS Block >> S101:{S101} |W601:{W601} \nT Block >> Z101:{Z101} |U301:{U301}";
        }
        public static string productionFormattedUpdate_message(Dictionary<string,Dictionary<string, int>> result){
            string BlockName="Q Block";
            Dictionary<string, int> QBlock = result.ContainsKey(BlockName)? result[BlockName] : new Dictionary<string, int>();
            int LTV=0 ;
            int MPV=0;
            QBlock.TryGetValue("LTV",out LTV);
            QBlock.TryGetValue("MPV",out MPV);
            BlockName="P Block";
            Dictionary<string, int> PBlock =  result.ContainsKey(BlockName)? result[BlockName] : new Dictionary<string, int>();
            int HCV=0 ;
            int ICV=0;
            int LCV=0;
            PBlock.TryGetValue("HCV",out HCV);
            PBlock.TryGetValue("ICV",out ICV);
            PBlock.TryGetValue("LCV",out LCV);
            BlockName="R Block";
            Dictionary<string, int> RBlock = result.ContainsKey(BlockName)? result[BlockName] : new Dictionary<string, int>();
            int BLR=0 ;
            int BLRPICKUP=0;
            int BLR108=0;
            RBlock.TryGetValue("BLR",out BLR);
            RBlock.TryGetValue("BLRPICKUP",out BLRPICKUP);
            RBlock.TryGetValue("BLR108",out BLR108);
            BlockName="S Block";
            Dictionary<string, int> SBlock = result.ContainsKey(BlockName)? result[BlockName] : new Dictionary<string, int>();
            int S101=0 ;
            int W601=0;
            SBlock.TryGetValue("S101",out S101);
            SBlock.TryGetValue("W601",out W601);
            BlockName="T Block";
            Dictionary<string, int> TBlock = result.ContainsKey(BlockName)? result[BlockName] : new Dictionary<string, int>();
            int Z101=0 ;
            int U301=0;
            TBlock.TryGetValue("Z101",out Z101);
            TBlock.TryGetValue("U301",out U301);
            //string building 
            int count = 0;
            DateTime corrected = DateTime.Now.Add(new TimeSpan(0,0,0));//to setup manual correction if wrong date time setup on the server
            string dateTitle=corrected.ToLongDateString();
            string TimeTillDay = corrected.ToString("hh:mm tt");
            string monospace = "```";
            string divider =     "+---+---+---------+---+----+\n";
            string header =      "|BLK|SHP| PLATFRM |ACT| TOT|\n";
            string main = monospace+"\n"
                                +dateTitle+"\n"
                                +"\n" //gap line 
                                +$"Production Status till {TimeTillDay}"+"\n"
                                +"\n" //gap line 
                                +divider + header + divider 
                              + $"| Q |TCF|  LTV    |{Templates.constSpace(LTV)}| {Templates.constSpace(LTV+MPV)}|"+"\n"
                              + $"|   |   |  MPV    |{Templates.constSpace(MPV)}|    |"+"\n"
                              +divider 
                              + $"| P |TCF|  HCV    |{Templates.constSpace(HCV)}| {Templates.constSpace(HCV+ICV+LCV)}|"+"\n"
                              + $"|   |   |  ICV    |{Templates.constSpace(ICV)}|    |"+"\n"
                              + $"|   |   |  LCV    |{Templates.constSpace(LCV)}|    |"+"\n"
                              + divider 
                              + $"| R |TCF|  BLR    |{Templates.constSpace(BLR)}| {Templates.constSpace(BLR+BLRPICKUP+BLR108)}|"+"\n"
                              + $"|   |   |BLRPICKUP|{Templates.constSpace(BLRPICKUP)}|    |"+"\n"
                              + $"|   |   |  BLR108 |{Templates.constSpace(BLR108)}|    |"+"\n"
                              +divider
                              + $"| S |TCF|  S101   |{Templates.constSpace(S101)}| {Templates.constSpace(S101+W601)}|"+"\n"
                              + $"|   |   |  W601   |{Templates.constSpace(W601)}|    |"+"\n"
                              +divider
                              + $"| T |TCF|  Z101   |{Templates.constSpace(Z101)}| {Templates.constSpace(Z101+U301)}|"+"\n"
                              + $"|   |   |  U301   |{Templates.constSpace(U301)}|    |"+"\n"
                              +divider+monospace;
            Console.Write("\n"+main+"\n");
            return main;
        }
        public static string constSpace(int count){
            int FixWidth=3;
            string temp = $"{count}";
            int length = temp.Length;
            return temp+String.Concat(Enumerable.Repeat(" ", FixWidth-length));

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
        public void updateEscalationStatus(string deviceID, string status, long escalationState) ;
        public void insertEscalation(string deviceID, string status, long escalationState);
        public void deleteEscalation(string deviceID);
        public void updateEscalation(string deviceID, string status, int type, long escalationState);
        public List<Tuple<long,string>> GetUserAlertGroup(string group );
        public List<object[]> GetPendingNotifications();

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
            Console.WriteLine("dude notification sent");// pending
            return ;
        } 

        public static void sendProductionUpdate(
            string Notification,
            ImessageClient client, 
            IDbModel model, 
             IlogWriter logger, 
             string group)
        {
            //get user in the group
            List<Tuple<long,string>> users = model.GetUserInProductionGroup(group);
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
            Console.WriteLine("production notification sent");// pending
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