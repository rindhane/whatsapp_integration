using System;
using dbSetup;
using System.Collections.Generic;
using LoggingLib;
using ProductionService;
using ChartingLib;
using System.Threading.Tasks;

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

        public object[] GetUserIdRegistration(int userID);
        public object[] GetRegistrationDetails (string userPhone);
        public void DeleteRegistration(long userID);
        public object[] GetUserDetails(string userPhone);
        public void AddUser(object[] userDetails);
        public List<Tuple<long,string>> GetUserInGroup(string group );

        public object[] GetSession(long userId);
        public void generateSession(MessageRecord record, long userId, int category, int stage);

        public void updateSession (MessageRecord record, object[] parentSession, int nextStage );
        public void clearSession (string parentSessionId);
        public void messageSent(MessageRecord record);
        public void messageReceived(UserMessageContainer response, string DialogueID);    
        public void updateStatus(Status status);       

    }

    public interface ImessageGenerator {

        public string extractContent(UserMessageContainer response);
        public bool IsacceptableResponse(string arg);
        public Message message(string[] args);

        public string[] backend (IHostDetails host, IProductionFetcher productionDb);
        public int nextStage();
        }
    
    public class Cat0_stage0:ImessageGenerator{
        
        private List<string> responses= new List<string> {"Yes","Stop"};
        public bool? register{get;set;}
        public string ?storedResponse;
        
        public string extractContent(UserMessageContainer response){
            storedResponse=response.ButtonText;
            return storedResponse;
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
        public string[] backend (IHostDetails host, IProductionFetcher productionDb) {
            if (storedResponse==responses[0])//for yes response
            {
                register=true;
                return new string[]{"Yes"};
            } 
            register=false;
            return new string[]{"Stop"}; //only other option
        }

        public int nextStage(){
            return register==true ? 1 : 0;
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
        
        public void createChart(string appRoot,
                                string picPath, 
                                List<Tuple<String,int>> data)
        {
            //inputs
            string exePath=appRoot+@"chartLib\chartLib.exe";
            string arg=pycharter.CreateArgString(picPath,data);
            Console.WriteLine($"arguments passed are ${arg}");
            //executing the charter executable
            new pycharter(exePath, arg).buildChart();
        }
        public Message message(string[] args){

            if (path_select.Equals("1")){
                //fetch data from production service
                //example data creation
                var data=new List<Tuple<String,int>>();
                data.Add(new Tuple<string,int>("A",51));
                data.Add(new Tuple<string,int>("B",61));
                data.Add(new Tuple<string,int>("C",41));
                data.Add(new Tuple<string,int>("D",48));       
                //from data create imagePlot
                string imgRelPath="/production.jpg";
                string imgCompletePath=args[2]+imgRelPath;
                createChart(appRoot:args[1], picPath:imgCompletePath,data:data);
                //message creation and return it
                Message message=new Message();
                message.content=null;
                message.url=args[0]+imgRelPath;
                Task.Delay(1000*20);// to make it wait for the chart preparation time of 20 secs
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

        public string[] backend (IHostDetails host, IProductionFetcher productionDb) {
            string PUBLIC_ADDRESS=host.PUBLIC_ADDRESS;
            return new string[]{PUBLIC_ADDRESS,host.appRoot,host.webRoot};
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

    //category to handle notification replies
    public class Cat1_stage1:ImessageGenerator{
        
        private List<string> responses= new List<string> {"Noted", "Ignore", "Taking-Action"};
        private string _selection;
        
        public string extractContent(UserMessageContainer response){
            _selection=response.ButtonText;
            return _selection;
        }
        public bool IsacceptableResponse(string arg){
            
            return responses.Contains(arg);
        } 
        public Message message(string[] args){
            Message message=new Message();
            message.content=Templates.Cat1_stage1(args[0]);
            message.url=null;
            return message;
        }

        public string[] backend (IHostDetails host, IProductionFetcher productionDb) {
            return new string[]{_selection};
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

        public string[] backend (IHostDetails host, IProductionFetcher productionDb) {
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

        public string[] backend (IHostDetails host, IProductionFetcher productionDb) {
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

        public static void sendOptInMessage(ImessageClient client,
                                            IDbModel model,int userID)
        {
            object [] userDetails=model.GetUserIdRegistration(userID);
            if (userDetails!=null)
            {
            MessageRecord messageRecord = client.sendMessage((string)userDetails[2],
                                                             Templates.Cat0_stage0_optInMessage((string) userDetails[1]), 
                                                             null);
            messageRecord.DialogueID=GenerateDialogId().ToString();
            messageRecord.KIND="Outgoing";
            model.messageSent(messageRecord);
            int category = 0;
            int stage = 0;
            model.generateSession(messageRecord,(long) userID, category, stage);
            }        
        }

        public static void handleRegistration(UserMessageContainer IncomingResponse, 
                                                ImessageClient client,
                                                IDbModel model,
                                                string userPhone,
                                                IHostDetails host,  
                                                IProductionFetcher productionDb)
        {
            object[] userDetails= model.GetRegistrationDetails(userPhone);
            object[] parentSession= GetSession((long)userDetails[0],model);
            ImessageGenerator messageResponse=new Cat0_stage0();
            MessageRecord messageRecord;
            if (messageResponse.IsacceptableResponse(messageResponse.extractContent(IncomingResponse)))
            {
                Message message=messageResponse.message(
                    messageResponse.backend(host,productionDb)
                    );
                if (messageResponse.nextStage()==1)
                {
                    Console.WriteLine("yes branch was execute");
                    model.messageReceived(IncomingResponse, (string)parentSession[4]);
                    
                    messageRecord = client.sendMessage((string)userDetails[2],
                                                             message.content, 
                                                             null);
                    messageRecord.DialogueID=(string)parentSession[4];
                    messageRecord.KIND="Outgoing";
                    model.messageSent(messageRecord);
                    model.AddUser(userDetails);
                    model.DeleteRegistration((long)userDetails[0]);
                    model.clearSession((string)parentSession[0]);
                    return ;
                }
                //action for stop message
                Console.WriteLine("Stop branch was execute");
                //model.messageReceived(IncomingResponse, (string)parentSession[4]);
                messageRecord = client.sendMessage((string)userDetails[2],
                                                             message.content, 
                                                             null);
                messageRecord.DialogueID=(string)parentSession[4];
                messageRecord.KIND="Outgoing";
                model.messageSent(messageRecord);
                return;
            }
            //arbitary response
            //model.messageReceived(IncomingResponse, (string)parentSession[4]);
            Console.WriteLine("abrupt branch was execute");
            messageRecord = client.sendMessage(userPhone, Templates.wrongResponse(), null);
            messageRecord.DialogueID=(string)parentSession[4];
            messageRecord.KIND="Outgoing";
            model.messageSent(messageRecord);
            return ;

        }
        public static void sendGreetingMessage(string userPhone, ImessageClient client, IDbModel model, 
                                                long userId,
                                                string dialogueID) 
        {
            object[]  userDetails= model.GetUserDetails(userPhone);
            MessageRecord messageRecord = client.sendMessage(userPhone,
                                                            Templates.Greeting_Message_renewed((string) userDetails[1]),
                                                             null);
            messageRecord.DialogueID=dialogueID ; 
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
                                                int nextStage, UserMessageContainer IncomingResponse)
        {
            model.messageReceived(IncomingResponse, (string)parentSession[4]) ;
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
            //get user in the group
            List<Tuple<long,string>> users = model.GetUserInGroup(group);
            foreach(Tuple<long,string> userDetails in users)
            {
                //sending the notifications
                MessageRecord message=client.sendMessage(userDetails.Item2 , Notification , null);
                message.DialogueID=GenerateDialogId().ToString();
                message.KIND="Outgoing";
                model.messageSent(message);
                object[] session = GetSession(userDetails.Item1 ,model);
                //clearing existing session
                if (session!=null)
                {
                    model.clearSession((string)session[0]); //based on SessionID column index in sessionstatus in record database
                }
                //this session is part of category 1
                int cat =1;
                int stage =1;
                model.generateSession(message, userDetails.Item1,cat, stage);
                return ;
            } 
            //logger.writeNotification($"Message sent {message.MessageID} on notification about {dat.device}");
            Console.WriteLine("it runs");// pending
        }
        public static void abruptResponse(string userPhone, 
                                                ImessageClient client,
                                                IDbModel model,
                                                object[] session, UserMessageContainer response)
        {
            model.messageReceived(response, (string) session[4]);
            MessageRecord messageRecord = client.sendMessage(userPhone, Templates.wrongResponse(), null);
            messageRecord.DialogueID=(string) session[4];
            messageRecord.KIND="Outgoing";
            model.messageSent(messageRecord);
            model.clearSession((string) session[0]);
        }
        public static async Task<string> ConversationHandler (UserMessageContainer response, ImessageClient client, 
                                                IDbModel model, IHostDetails host,  
                                                IProductionFetcher productionDb) {
            //authenticate user
            string userPhone="+"+response.WaId;
            if (!IsUser(userPhone,model)) {
                //check if in Registration-Table
                object[] user=model.GetRegistrationDetails(userPhone);
                if(user!=null)
                {
                    handleRegistration(response,client,model,userPhone,host,productionDb);
                    return null;
                }
                //send no authorization message;
                string DialogueID=GenerateDialogId().ToString();
                model.messageReceived(response, DialogueID);
                MessageRecord messageRecord = client.sendMessage(userPhone, Templates.noAuth(), null);
                messageRecord.DialogueID=DialogueID;
                messageRecord.KIND="Outgoing";
                model.messageSent(messageRecord);
                return null;
            }
            //after authentication, check is there existing session
            //get userID
            object[] userDetails = GetUserDetails(userPhone,model);
            long userId=(long)userDetails[0];
            //from userID get sessionID;
            object[] result= GetSession(userId,model);
            //if null send greeting Message
            if (result==null){
                string dialogueID = GenerateDialogId().ToString();
                model.messageReceived(response, dialogueID);
                sendGreetingMessage(userPhone,client, model, userId,dialogueID);
                return null ;
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
                    abruptResponse(userPhone,client, model, result, response );
                    return null;
                }
            }
            //run action function on the selected path
            if (messageResponse.IsacceptableResponse(messageResponse.extractContent(response))){
                Message message=messageResponse.message(
                    messageResponse.backend(host,productionDb)
                    );
                int nextStage=messageResponse.nextStage();
                sendResponseMessage(userPhone, client, model, userDetails, result , message.content, message.url, 
                                    nextStage, response);
                return null;
            }
            //reply for abrupt response messages
            abruptResponse(userPhone,client, model, result, response );
            return null;
        }
    }

}