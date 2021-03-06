
namespace WhatsappService
{
    public static class Templates {
        public static string noAuth(){
            return @"Thank you for joining in. You will start receiving notifications on approval from MES-admin.";
 
        }
        public static string Greeting_Message() {
            return
            @"Welcome to M&M MES Updates. Select your option : 
            (1) Consolidated Production Chart
            (2) Check Particular Shop Production";
        }
        public static string Greeting_Message_renewed(string name) {
            return
            $"Hi {name},\nWelcome to M&M MES Updates. Select your option : \n(1) Consolidated Production Chart\n(2) Check Particular Shop Production";
        }

        public static string Cat0_stage0_optInMessage(string name){
            return $"Hi {name},\nGreetings from the M&M MES Team. Please reply with : \nYes: To Receive the critical alerts from production systems\nStop : To stop receiving any alerts.";
        }
        public static string Cat0_stage0(string option){
            if (option.Equals("Yes"))
            {
                return @"Based on your confirmation, you have been subscribed to alerts"; 
            }
            if (option.Equals("Stop"))
            {
                return @"Based on your request, you remain unsubscribed"; 
            }
            return null;
        }
    
        public static string Cat1_stage0(){
            return @"Please enter a character from A to D to select one of the Block.";
        }

        //response to notification reply
        public static string Cat1_stage1(string lastResponse){
            return $"Thanks for the response. Your response: “{lastResponse}” has been recorded into the system.";
        }
        public static string Cat1_stage2(string block){
            return $"You have selected Block-{block.ToUpper()}, Please select one of the shop by entering their respective shop’s indicated number :\n Press-1; BIW-2; Paint-3; TCF-4";
        }
        public static string Cat1_stage3(string shop, int val){
            var ci = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            string stamp=System.DateTime.Now.ToString("hh:mm:ss on ddd d MMM ", ci);
            return $"The current production for {shop} is {val} by {stamp}";
        }

        public static string wrongResponse(){
            return @"Response was not understood. Please retry";
        }
        public static string notification_Message(string probe, string device, string status, string description) {
            return
            $"Critical Alert Notification:\nService {probe} on {device} is now {status} with the following issue {description}";
        }
    }   
    
}