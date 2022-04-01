using System;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace ProxyClient
{
    public class NTLMClient
    {

        private readonly HttpClientHandler handler ;
        private readonly HttpClient client;
        private string ProxyUsername;
        private string ProxyPassword;
        private string testUrl;
        public NTLMClient(string fileName, string test )
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
            //reference>>https://stackoverflow.com/questions/47822842/ntlm-authentication-httpclient-in-core
            var uri = new Uri(proxyUrl);
            var credentialsCache = new CredentialCache { { uri, "NTLM", CredentialCache.DefaultNetworkCredentials } }; 
            handler = new HttpClientHandler(){
               Credentials = credentialsCache  
            };
            client=new HttpClient(handler) {BaseAddress = uri };
        
        }

        public async Task getResponse()
        {
            //var byteArray = Encoding.ASCII.GetBytes($"{ProxyUsername}:{ProxyPassword}");
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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