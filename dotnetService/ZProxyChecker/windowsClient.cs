using System;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace ProxyClient
{

    public class WindowsClient
    {

        private readonly HttpClientHandler handler ;
        private readonly HttpClient client;
        private string ProxyUsername;
        private string ProxyPassword;
        private string testUrl;
        public WindowsClient(string fileName, string test )
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
            //var uri = new Uri(proxyUrl);
            var credCache = new CredentialCache { 
                                                    { 
                                                    new Uri(testUrl), "Negotiate", 
                                                    new NetworkCredential(ProxyUsername, ProxyPassword, "sts.mahindra.com") 
                                                    } 
                                                }; 
            CookieContainer cookieContainer = new System.Net.CookieContainer();
            handler = new HttpClientHandler()
            {
                Proxy = new WebProxy(proxyUrl),
                UseProxy = useProxy,
                UseDefaultCredentials = false,
                Credentials = credCache,
                AllowAutoRedirect = true,
                ClientCertificateOptions = ClientCertificateOption.Automatic,
                UseCookies = true,
                CookieContainer = cookieContainer,  
            };
            
            //handler.UseCookies = true;
            client=new HttpClient(handler); //{BaseAddress = uri };
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.ExpectContinue = false;
            
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