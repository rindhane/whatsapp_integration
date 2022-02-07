using System.Text.RegularExpressions;
namespace FileWatcherService {
    
    public class resultParams{
        
        public string start{get;set;}
        public string probe{get;set;}
        public string device{get;set;}
        public string status{get;set;}
        public string description{get;set;}

    }
    public class RegexContentExtractor{
        public Regex regexPattern{get;set;}
        public string regString{get;set;}=
        @"(?<start>[0-9.\-:<>\s]+)\sService (?<probe>.*?) on (?<device>.*?) is now (?<status>.*?) \((?<description>.*?)\)";

        public MatchCollection matches;
        
        public RegexContentExtractor(){
            regexPattern=new Regex(regString);
        }
        public resultParams getParams(string text){
            resultParams result = new resultParams();
            Match temp=regexPattern.Match(text);
            result.start = (temp.Groups["start"]).ToString();
            result.probe = (temp.Groups["probe"]).ToString();
            result.device = (temp.Groups["device"]).ToString();
            result.status = (temp.Groups["status"]).ToString();
            result.description = (temp.Groups["description"]).ToString();
            //System.Console.WriteLine($"0:{result.start},1:{result.probe}, 2:{result.device},3:{result.status},4:{result.description}");
            return result;
        }
    }

    public class RegexFileSelector{

        public Regex regexPattern{get;set;}
        public string regString{get;set;}

        public MatchCollection matches;
        
        public RegexFileSelector(){
            var ci = System.Globalization.CultureInfo.CreateSpecificCulture("en-IN");
            string stamp=System.DateTime.Now.ToString("yyyy.MM.dd", ci);
            regString=$"Syslog-{stamp}-[0-9.]+\\.log$";
            regexPattern=new Regex(regString);
        }
        public string cleanName(string path){
            string[] breaks = path.Split(@"\");
            return breaks[^1];
        }

        public string findMatch(string name){
            Match temp =regexPattern.Match(name);
            return temp.Value;
        }

        public string findFile(string path ){
            string name = cleanName(path);
            string file = findMatch(name);
            return file;
        }


    }

}