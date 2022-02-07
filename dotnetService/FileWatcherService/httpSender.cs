using System.Threading.Tasks;
using System.Net.Http;
namespace FileWatcherService
{
    public class httpSender
    {        
        private readonly HttpClient _httpClient;
        public string? NotificationUrl;
        public httpSender ()
        {
             //NotificationUrl=url;
            _httpClient=new HttpClient();
            _httpClient.Timeout = System.TimeSpan.FromMinutes(2);
        }
        public async Task sendNotification(string bodyText)
        {
            try
            {
            //url preparation
            StringContent body = new StringContent(bodyText);
            //System.Console.WriteLine(body.ToString());
            //send notification
            HttpResponseMessage response = await _httpClient.PostAsync(NotificationUrl, body);
            response.EnsureSuccessStatusCode();
            string responseBody=await response.Content.ReadAsStringAsync();
            System.Console.WriteLine(responseBody); 
            }
            catch(HttpRequestException e) 
            {
                //Console.WriteLine("\nException Caught!");
                System.Console.WriteLine("MessageErro:{0}", e.Message);     
            }
        }
    }
    
}