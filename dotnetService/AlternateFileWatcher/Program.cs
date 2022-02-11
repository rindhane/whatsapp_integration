using System;
using System.IO;

namespace TrialSpace {
    public static class  MainProgram {
        static void Main() 
        {
            var watcher = new FileSystemWatcher(@"testSample");
            watcher.Filter = "dudeSample1.txt";
            //watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;
            watcher.Changed += OnChanged;

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        public static void Reader(string path){
            Console.WriteLine($"string provided: {path}");
            StreamReader file = File.OpenText(path);
            for (int i=0; i<Counter.TotalCount(); i++){
                file.ReadLine();
            }
            string line ;
            while ((line=file.ReadLine())!=null){
                Console.WriteLine($"Read : {line}");
                Counter temp=new Counter();
            }
            file.Close();
        }
        private static void OnChanged(object sender, FileSystemEventArgs e) {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath} {e.ChangeType}");
            Reader(e.FullPath);
            
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