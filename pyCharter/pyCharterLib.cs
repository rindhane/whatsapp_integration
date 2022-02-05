using System.Diagnostics;
using System;

namespace ChartingLib
{
    public class pycharter
    { 
        public ProcessStartInfo chartProcessInfo ;
        //ref doc : https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process?view=net-6.0
        //ref : https://stackoverflow.com/questions/9679375/how-can-i-run-an-exe-file-from-my-c-sharp-code
        //ref doc: read output : https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.beginoutputreadline?view=net-6.0s
        public pycharter (string filePath, string args )
        {
            chartProcessInfo = new ProcessStartInfo();
            chartProcessInfo.FileName=filePath;
            chartProcessInfo.UseShellExecute = false;
            chartProcessInfo.CreateNoWindow=true;
            //chartProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;
            chartProcessInfo.Arguments=args;

        }

        public void buildChart()
        {
            try {
                Process.Start(chartProcessInfo);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            

        } 

        
    }
}