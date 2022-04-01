using System;
using System.Threading.Tasks;
using AdfsSaml;

namespace ProxyClient
{
    public class MainProgram
    {
       
        public static async Task Main(string[] args)
        {
            
            Console.WriteLine("Testing SAMLTOKENCLIENT");
            Uri ServerURI= new Uri("https://sts.mahindra.com");
            SamlResponse response = new SamlResponse(ServerURI,false);
            string[] result=response.GetReliantPartyCollection();
            foreach(string index in result)
            {
                System.Console.Write($"{index}:");
                try{
                    System.Console.Write(response.RequestSamlResponse(index)+"\n");
                }catch(Exception e){
                    System.Console.Write(e.Message+"\n");
                }
            }      
            Console.WriteLine("Testing Windowsactivatedclient Proxy Client!");
            try {
                WindowsClient client3=new WindowsClient(args[0], args[1]);
                await client3.getResponse();
            }catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public static void test()
        {
            /*
            var tokenParams = WSTrustTokenParameters.CreateWSFederationTokenParameters(Binding issuerBinding, EndpointAddress issuerAddress);
            var clientCredentials = new ClientCredentials();
            clientCredentials.UserName.UserName = "user";
            clientCredentials.UserName.Password= "its_a_secret";
            var trustCredentials = new WSTrustChannelClientCredentials(clientCredentials);
            var tokenManager = trustCredentials.CreateSecurityTokenManager();
            var tokenRequirement = new SecurityTokenRequirement();
            tokenRequirement.Properties["http://schemas.microsoft.com/ws/2006/05/servicemodel/securitytokenrequirement/IssuedSecurityTokenParameters"] = tokenParams;
            var tokenProvider = tokenManager.CreateSecurityTokenProvider(tokenRequirement);
            */
        } 

    }

}
