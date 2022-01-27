using System;
using dbSetup;

namespace WhatsappService {

    public interface ImessageClient {
        public MessageRecord sendMessage(string phone, string newMessage, string url); //implements the message-client-api for sending the message
    }

    public interface IDbModel {
        public bool IsUser(string userPhone) ;
        public long GetUserId(string userPhone);

        public object[] GetSession(long userId);
        public void messageSent(MessageRecord record);
        public void generateSession(MessageRecord record, long userId, int category);

    }

    public static class DialogFlow {

        public static bool IsUser(string userPhone, IDbModel model){
            return model.IsUser(userPhone);
        }
        public static long GetUserId(string userPhone, IDbModel model){
            return model.GetUserId(userPhone);
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
        public static void sendGreetingMessage(string userPhone, ImessageClient client, IDbModel model, long userId) {
            MessageRecord messageRecord = client.sendMessage(userPhone,Templates.Greeting_Message(), null);
            messageRecord.DialogueID=GenerateDialogId().ToString();
            messageRecord.KIND="Outgoing";
            model.messageSent(messageRecord);
            int category = 1;
            model.generateSession(messageRecord,userId, category);
        }

               
        public static void ConversationHandler (UserMessageContainer response, ImessageClient client, IDbModel model ) {
            //authenticate user
            string userPhone="+"+response.WaId;
            bool check = IsUser(userPhone,model);
            if (!check) {
                //send no authorization message;
                client.sendMessage(userPhone, Templates.noAuth(), null);
                return ;
            }
            //after authentication, check is there existing session
            //get userID
            long userId = GetUserId(userPhone,model);
            //System.Console.WriteLine(userId); 
            //from userID get sessionID;
            object[] result= GetSession(userId,model);
            //if null send greeting Message
            if (result==null){
                sendGreetingMessage(userPhone,client, model, userId);
                return ;
            }
            //if sessionID get cat & stage
            long cat=(long) result[3];
            Console.WriteLine($"cat: {cat}");
            Console.WriteLine($"cat: {result[4]}");

            
            // from cat & stage get the action function 
            //run action function and run the output


        }
    }

}