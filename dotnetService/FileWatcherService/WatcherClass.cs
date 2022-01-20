using System.Net.Http.Json;
using System.Text.Json;
using System.IO;

namespace FileWatcherService {

public class WatcherService
{

    private readonly HttpClient _httpClient;
    private const string DudeFilePath=@"..\testSample\dudeSample1.txt";
    private readonly JsonSerializerOptions _options = new() {
        PropertyNameCaseInsensitive = true
    };
    //config for WhatsAppService listener url
    private const string NotificationUrl="http://localhost:7600/notification";//http://localhost:8080/notification
    public WatcherService(HttpClient httpClient) => _httpClient=httpClient;
    public async Task<string> GetNotificationAsync()
    {
        await Reader(DudeFilePath); 
        return $"Read the file {DateTime.Now.ToString()}";
    }
    public async Task Reader(string path){
            Console.WriteLine($"string provided: {path}");
            StreamReader file = File.OpenText(path);
            for (int i=0; i<Counter.TotalCount(); i++){
                await file.ReadLineAsync();
                Console.WriteLine($"Skip Read: {Counter.TotalCount()}");
            }
            string line ;
            while ((line=await file.ReadLineAsync())!=null){
                Console.WriteLine($"Read : {line}");
                await sendNotification(line.Substring(0,5)); 
                Counter temp=new Counter();
            }
            file.Close();
        }

    public async Task sendNotification(string line){
        try
        {
        //url preparation
        string url_with_query=NotificationUrl + $"?name={line}";
        //send notification
          HttpResponseMessage response = await _httpClient.GetAsync(url_with_query);
          response.EnsureSuccessStatusCode();
          string responseBody=await response.Content.ReadAsStringAsync();
          Console.WriteLine(responseBody); 
        }
        catch(HttpRequestException e) 
        {
            //Console.WriteLine("\nException Caught!");
            Console.WriteLine("MessageErro:{0}", e.Message);     
        }
    }
    
}
public class Counter
{
      
        // The static variable count is used to store
        // the count of objects created.
        static int count = 0;
        
        // Constructor of the class, in which count
        // value will be incremented
        public Counter() 
        { 
            count++; 
        }      
        // Method totalcount is used to return 
        // the count variable.
        public static int TotalCount() 
        {  
            return count; 
        }
}


}