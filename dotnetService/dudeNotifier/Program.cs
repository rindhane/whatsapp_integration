using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
namespace DudeConnection {

class dudeNotifier {
    static async Task Main (string[] args) 
    {
        //Settings settings=await readSpecs("settings.json");
        httpSender client = new httpSender (args[0]);
        string body = $"{args[1]};{args[2]};{args[3]};{args[4]};{args[5]};";
        await client.sendNotification(body);
    }

    public static async Task<Settings> readSpecs(string filePath)
    {

        StreamReader file = File.OpenText(filePath);
        string text = await file.ReadToEndAsync();
        return JsonConvert.DeserializeObject<Settings>(text);
    } 
}

class Settings{
    public string receiverUrl{get; set;}
    public string machineName{get;set;} 
}

}
