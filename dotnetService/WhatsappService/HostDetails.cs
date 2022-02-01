namespace WhatsappService {

    public interface IHostDetails{
        //public address of the webservice
        public string PUBLIC_ADDRESS{get;set;}
        //temp mobile to test receiving the message
        public string? TEMP{get;set;}
    }

    //contains the configurations
    public class HostDetails : IHostDetails{
            //public address of the webservice
            public string PUBLIC_ADDRESS{get;set;}
            //temp mobile to test receiving the message
            public string? TEMP{get;set;}
            public HostDetails(){
                
                PUBLIC_ADDRESS=System.Environment.GetEnvironmentVariable("PUBLIC_ADDRESS");
                
                TEMP=System.Environment.GetEnvironmentVariable("TEST_NUMBER");
            }
        }
    
}