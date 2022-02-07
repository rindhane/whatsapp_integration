using System.Text.Json;
using System.IO;
using System.Threading.Tasks;

namespace FileWatcherService {

    public class WatcherService
    {

        private readonly httpSender _httpClient;
        //primary inputs for filename & api url to send notification 
        private const string DudeFilePath=@"..\testSample\dudeSample1.txt";
        private const string NotificationUrl="http://localhost:8080/notification";
        //settings for JsonSerialize
        private readonly JsonSerializerOptions _options = new() {
            PropertyNameCaseInsensitive = true
        };
        
        public WatcherService(httpSender sender) 
        {
            _httpClient=sender;
            _httpClient.NotificationUrl=NotificationUrl;

        }
        public async Task<string> GetNotificationAsync()
        {
            //write function to find the file among the folder
            await Reader(DudeFilePath); 
            return $"Read the file {System.DateTime.Now.ToString()}";
        }
        public async Task Reader(string path){
                System.Console.WriteLine($"string provided: {path}");
                StreamReader file = File.OpenText(path);
                for (int i=0; i<Counter.TotalCount(); i++){
                    await file.ReadLineAsync();
                    System.Console.WriteLine($"Skip Read: {Counter.TotalCount()}");
                }
                string line ;
                while ((line=await file.ReadLineAsync())!=null){
                    System.Console.WriteLine($"Read : {line}");
                    //extract certain values from the parameters of the line
                    await _httpClient.sendNotification(line.Substring(0,5)); 
                    Counter temp=new Counter();
                }
                file.Close();
            }
        
    }
    public class Counter
    {
        
        // The static variable count is used to store
        // the count of lines previously read
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