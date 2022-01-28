
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
         public static string Cat0_stage0(string name){
            return $"Hi {name},\nGreetings from the M&M MES Team. Please reply with :\nYes: To Receive the critical alerts from production systems\nStop : To stop receiving any alerts.";
        }
    
        public static string Cat1_stage0(){
            return @"Please enter a character from A to D to select one of the Block.";
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
    }   
    
}