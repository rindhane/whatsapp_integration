
namespace WhatsappService
{
    public static class Templates {
        public static string noAuth(){
            return @"Thank you for joining in. You will start receiving notifications on approval from MES-admin ";
        }
        public static string Greeting_Message() {
            return
            @"Welcome to M&M MES Updates. Select your option : 
            (1) Consolidated Production Chart
            (2) Check Particular Shop Production";
        }
    }
    
}