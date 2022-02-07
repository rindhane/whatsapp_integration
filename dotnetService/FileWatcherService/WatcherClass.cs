using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace FileWatcherService {

public class WatcherService
{

    private readonly httpSender _httpClient;
    //primary inputs for filename & api url to send notification 
    //private const string DudeFilePath=@"..\testSample\Dude_Log.txt";
    public string DudeFolderPath{get;set;}
    public string NotificationUrl{get;set;}
    public string ServiceName{get;set;}
    //settings for JsonSerialize
    private readonly JsonSerializerOptions _options = new() {
        PropertyNameCaseInsensitive = true
    };
    public WatcherService(httpSender sender, string URL) 
    {
        _httpClient=sender;
        _httpClient.NotificationUrl=URL;

    }

    
    public string dudeFilePicker(){
        RegexFileSelector selector = new RegexFileSelector();
        var files= Directory.EnumerateFiles(DudeFolderPath,"*.log");
        foreach (string file in files){
            if(!System.String.IsNullOrEmpty(selector.findFile(file))){
                return file; 
            }
        }
        return null;
    }
    
    public async Task GetNotificationAsync(string DudeFilePath, string DateString)
    {
        
        await foreach (resultParams obj in Reader(DudeFilePath, DateString))
        {
            await _httpClient.sendNotification(bodyFromParams(obj));
        }
    }
    public async IAsyncEnumerable<resultParams> Reader(string filePath, string DateString)
    {
        int buffer=1024;
        FileStream fs= new FileStream(filePath,
                                        FileMode.Open,
                                        FileAccess.Read,
                                        FileShare.ReadWrite,
                                        buffer, FileOptions.Asynchronous);
        string line;
        StreamReader file = new StreamReader(fs, Encoding.UTF8);
        while (DateString.Equals(System.DateTime.Now.ToShortDateString()))
            {
                if ((line=await file.ReadLineAsync())!=null){
                    yield return PatternExtractor(line);
                }
            }
    }

    public resultParams PatternExtractor(string line)
        {
            RegexContentExtractor regPattern = new RegexContentExtractor();
            resultParams result = regPattern.getParams(line);
            //System.Console.WriteLine($"0:{result.start},1:{result.probe}, 2:{result.device},3:{result.status},4:{result.description}");
            return result;
        }
    
    public string bodyFromParams(resultParams obj)
        {
            
            string body = $"{obj.probe};{obj.device};{obj.status};{obj.description};{ServiceName};";
            return body;
        }
    
}
    

}