using System;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
//using Microsoft.IdentityModel.Protocols.WSTrust;

namespace ProxyClient
{

    public class SAMLClient
    {

        private readonly HttpClientHandler handler ;
        private readonly HttpClient client;
        private string ProxyUsername;
        private string ProxyPassword;
        private string testUrl;
        public SAMLClient(string fileName, string test )
        {
            string FileName=fileName;
            var reader= new StreamReader(FileName);
            string? proxyUrl=reader.ReadLine().Split("=")[1];//Configuration["Configs:Proxy_Details:proxyUrl"];
            ProxyUsername=reader.ReadLine().Split("=")[1];//Configuration["Configs:Proxy_Details:username"];
            ProxyPassword=reader.ReadLine().Split("=")[1];//Configuration["Configs:Proxy_Details:password"];
            testUrl=reader.ReadLine().Split("=")[1];
            bool useProxy =true;
            if (test.Equals("show")) 
            {
                Console.WriteLine(proxyUrl+"|"+ProxyUsername+"|"+ProxyPassword);
            }
            reader.Close();
            //creating client
            var uri = new Uri(proxyUrl); 
            handler = new HttpClientHandler(){
               UseDefaultCredentials=true,
               AllowAutoRedirect = false  
            };
            handler.CookieContainer = new System.Net.CookieContainer();
            handler.UseCookies = true;
            client=new HttpClient(handler) {BaseAddress = uri };
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.ExpectContinue = false;
            var stsEndpoint = "https://sts.mahindra.com/adfs/services/trust/13/UsernameMixed";
            var reliantPartyUri = proxyUrl;
            /*
            var factory = new Microsoft.IdentityModel.Protocols.WSTrust.WSTrustChannelFactory(
            new UserNameWSTrustBinding(
                SecurityMode.TransportWithMessageCredential),
                new EndpointAddress(stsEndpoint)
                );
            */
            //factory.TrustVersion = System.ServiceModel.Security.TrustVersion.WSTrust13;
            //factory.Credentials.UserName.UserName = ;
            //factory.Credentials.UserName.Password = ;
            /*
            var rst = new RequestSecurityToken
            {
                RequestType = RequestTypes.Issue,
                AppliesTo = new EndpointAddress(reliantPartyUri),
                KeyType = KeyTypes.Bearer,
            };
            var channel = factory.CreateChannel();
            var token = channel.Issue(rst) as GenericXmlSecurityToken;
            var saml = token.TokenXml.OuterXml;
            System.Console.WriteLine(saml);
            */
        }
 

        public async Task getResponse()
        {
            //var byteArray = Encoding.ASCII.GetBytes($"{ProxyUsername}:{ProxyPassword}");
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(testUrl);
            HttpContent content = response.Content;

            // ... Check Status Code                                
            Console.WriteLine("Response StatusCode: " + (int)response.StatusCode);

            // ... Read the string.
            string result = await content.ReadAsStringAsync();

            // ... Display the result.
            if (result != null)
            {
                Console.WriteLine(result);
            }
        }


    
    }    

    
}