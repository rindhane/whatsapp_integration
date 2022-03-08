using System;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TransferClient
{

    public class SimpleClient
    {
        private string testUrl;
        private readonly HttpClientHandler handler ;
        private readonly HttpClient client;
        public SimpleClient( string URL)
        {
        
            testUrl=URL;
            //creating client 
            handler = new HttpClientHandler()
            {
                UseProxy = false
            };

            client=new HttpClient(handler);
            
        }

        public async Task getResponse()
        {
            //var byteArray = Encoding.ASCII.GetBytes($"{ProxyUsername}:{ProxyPassword}");
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
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
        public async Task postJson(string JsonString)
        {
            //var byteArray = Encoding.ASCII.GetBytes($"{ProxyUsername}:{ProxyPassword}");
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            //reference for sending the json string
            //https://stackoverflow.com/questions/6117101/posting-jsonobject-with-httpclient-from-web-api
            var jsonPayload = new StringContent(JsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(testUrl,jsonPayload);
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