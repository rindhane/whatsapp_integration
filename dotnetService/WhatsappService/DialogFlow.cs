using System;
using dbSetup;
using System.Collections.Generic;
using LoggingLib;

namespace WhatsappService {
    enum shops{
                Press,
                BIW,
                Paint,
                TCF
            }
    public class Message {
        public string content {get;set;}
        public string url{get;set;}
    }

    public interface ImessageClient {
        public MessageRecord sendMessage(string phone, string newMessage, string url); //implements the message-client-api for sending the message
    }

    public interface IDbModel {
        public bool IsUser(string userPhone) ;
        public object[] GetUserDetails(string userPhone);

        public object[] GetSession(long userId);
        public void messageSent(MessageRecord record);
        public void generateSession(MessageRecord record, long userId, int category, int stage);

        public void updateSession (MessageRecord record, object[] parentSession, int nextStage );

        public void clearSession (string parentSessionId);

    }

    public interface ImessageGenerator {

        public string extractContent(UserMessageContainer response);
        public bool IsacceptableResponse(string arg);
        public Message message(string[] args);

        public string[] backend ();
        public int nextStage();
        }
    
    public class Cat0_stage0:ImessageGenerator{
        
        private List<string> responses= new List<string> {"Yes","Stop"};
        
        public string extractContent(UserMessageContainer response){
            return response.ButtonText;
        }
        public bool IsacceptableResponse(string arg){
            
            return responses.Contains(arg);
        } 
        public Message message(string[] args){
            Message message=new Message();
            message.content=Templates.Cat0_stage0(args[0]);
            message.url=null;
            return message;
        }
        public string[] backend () {
            return new string[]{"",""};
        }

        public int nextStage(){
            return -1;
        }
    }
    public class Cat1_stage0:ImessageGenerator{
        private List<string> responses= new List<string> {"1","2"};
        private string path_select;
        
        public string extractContent(UserMessageContainer response){
            path_select=response.ButtonText;
            return path_select;
        }
        public bool IsacceptableResponse(string arg){
            
            return responses.Contains(arg);
        } 
        public Message message(string[] args){

            if (path_select.Equals("1")){
                Message message=new Message();
                string PUBLIC_ADDRESS=new HostDetails().PUBLIC_ADDRESS;
            message.content=null;
            message.url=PUBLIC_ADDRESS+"/index.jpg";
                return message;
            }
            if (path_select.Equals("2")){
                Message message=new Message();
                message.content=Templates.Cat1_stage0();
                message.url=null;
             return message;   
            }
            return new Message();
        }

        public string[] backend () {
            return new string[]{"",""};
        }
        public int nextStage(){
            if (path_select.Equals("1")) {
            return -1;
            }
            if (path_select.Equals("2")){
                return 2;
            }
            return 0;
        }
    }

    //not under utlization
    public class Cat1_stage1:ImessageGenerator{
        
        private List<string> responses= new List<string> {};
        private string _selection;
        private int production =50;
        
        public string extractContent(UserMessageContainer response){
            _selection=response.ButtonText;
            return _selection;
        }
        public bool IsacceptableResponse(string arg){
            
            return responses.Contains(arg);
        } 
        public Message message(string[] args){
            Message message=new Message();
            message.content=Templates.Cat1_stage3(args[0], production);
            message.url=null;
            return message;
        }

        public string[] backend () {
            return new string[]{"Trial"};
        }
        public int nextStage(){
            return -1;
        }
    }

    public class Cat1_stage2:ImessageGenerator{
        
        private List<string> responses= new List<string> {"a","b","c","d"};
        private string _selection;
        
        public string extractContent(UserMessageContainer response){
            _selection=response.Body.ToLower();
            return _selection;
        }
        public bool IsacceptableResponse(string arg){
            
            return responses.Contains(arg);
        } 
        public Message message(string[] args){
            Message message=new Message();
            message.content=Templates.Cat1_stage2(args[0]);
            message.url=null;
            return message;
        }

        public string[] backend () {
            return new string[]{_selection};
        }
        public int nextStage(){
            return 3;
        }
    }

    public class Cat1_stage3:ImessageGenerator{
        
        private List<string> responses= new List<string> {"1","2","3","4"};
        private string _selection;
        private int production;
        
        public string extractContent(UserMessageContainer response){
            _selection=response.Body.ToLower();
            return _selection;
        }
        public bool IsacceptableResponse(string arg){
            
            return responses.Contains(arg);
        } 
        public Message message(string[] args){
            Message message=new Message();
            message.content=Templates.Cat1_stage3(args[0], production);
            message.url=null;
            return message;
        }

        public string[] backend () {
            string result=((shops)(int.Parse(_selection)-1)).ToString();
            production=50;
            return new string[]{result};
        }
        public int nextStage(){
            return -1;
        }
    }

    public static class DialogFlow {
        public static bool IsUser(string userPhone, IDbModel model){
            return model.IsUser(userPhone);
        }
        public static object[] GetUserDetails(string userPhone, IDbModel model){
            return model.GetUserDetails(userPhone);
        }
        public static object[] GetSession(long userId, IDbModel model){
            return model.GetSession(userId);
        }
        
        public static Guid GenerateDialogId(){
            return Guid.NewGuid();
        }
        public static Guid GenerateSessionId(){
            return Guid.NewGuid();
        }

        public static void sendOptInMessage(){
            return ;
        }
        public static void sendGreetingMessage(string userPhone, ImessageClient client, IDbModel model, long userId) {
            MessageRecord messageRecord = client.sendMessage(userPhone,Templates.Greeting_Message(), null);
            messageRecord.DialogueID=GenerateDialogId().ToString();
            messageRecord.KIND="Outgoing";
            model.messageSent(messageRecord);
            int category = 1;
            int stage = 0;
            model.generateSession(messageRecord,userId, category, stage);
        }

        public static void sendResponseMessage(string userPhone, 
                                                ImessageClient client,
                                                IDbModel model, 
                                                object[] userDetails,
                                                object [] parentSession,
                                                string message, 
                                                string url,
                                                int nextStage)
        {
            MessageRecord messageRecord = client.sendMessage(userPhone, message, url);
            messageRecord.DialogueID=(string) parentSession[4];
            messageRecord.KIND="Outgoing";
            model.messageSent(messageRecord);
            if (nextStage!=-1) {
            model.updateSession(messageRecord,parentSession,nextStage);
            return ;
            }
            model.clearSession((string) parentSession[0]);
            return;
        }
        public static void sendNotificationMessage(string Notification ,
                                                  ImessageClient client,
                                                  IDbModel model, 
                                                  IlogWriter logger,
                                                  string group)
        {
            //
            //MessageRecord message=client.sendMessage(host.TEMP, $"This is message of {dat.name}", null); 
            //logger.writeNotification($"Message sent {message.MessageID} on notification about {dat.device}");
            Console.WriteLine("it runs");// pending
        }
        public static void abruptResponse(string userPhone, 
                                                ImessageClient client,
                                                IDbModel model,
                                                string sessionId)
        {
            MessageRecord messageRecord = client.sendMessage(userPhone, Templates.wrongResponse(), null);
            model.clearSession(sessionId);
        }
        public static void ConversationHandler (UserMessageContainer response, ImessageClient client, IDbModel model ) {
            //authenticate user
            string userPhone="+"+response.WaId;
            if (!IsUser(userPhone,model)) {
                //send no authorization message;
                client.sendMessage(userPhone, Templates.noAuth(), null);
                return ;
            }
            //after authentication, check is there existing session
            //get userID
            object[] userDetails = GetUserDetails(userPhone,model);
            long userId=(long)userDetails[0];
            //from userID get sessionID;
            object[] result= GetSession(userId,model);
            //if null send greeting Message
            if (result==null){
                sendGreetingMessage(userPhone,client, model, userId);
                return ;
            }
            //if sessionID get cat & stage
            long cat=(long) result[2]; //obtaining category from result_session
            long stage=(long) result[3]; //obtaining stage
            Console.WriteLine($"cat:{result[2]} | stage: {result[3]} | dialogue: {result[4]}");
            // from cat & stage get the action function 
            ImessageGenerator messageResponse = null;
            if (cat==0){
                messageResponse=new Cat0_stage0();
            }
            if (cat==1){
                if (stage==0){
                    messageResponse=new Cat1_stage0();
                }
                if (stage==1)
                {
                    messageResponse=new Cat1_stage1();
                }
                if (stage==2)
                {
                    messageResponse=new Cat1_stage2();
                }
                if (stage==3)
                {
                    messageResponse=new Cat1_stage3();
                }
                if (stage!=0 && stage!=1 && stage!=2 && stage!=3 )
                {
                    abruptResponse(userPhone,client, model, (string) result[0] );
                    return ;
                }
            }
            //run action function on the selected path
            if (messageResponse.IsacceptableResponse(messageResponse.extractContent(response))){
                Message message=messageResponse.message(messageResponse.backend());
                int nextStage=messageResponse.nextStage();
                sendResponseMessage(userPhone, client, model, userDetails, result , message.content, message.url, nextStage);
                return ;
            }
            //reply for abrupt response messages
            abruptResponse(userPhone,client, model, (string) result[0] );
            return ;
        }
    }

}