using System.Diagnostics;
using System;
using System.Collections.Generic;

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
        public static string CreateArgString(string picPath,List<Tuple<string,int>>plotData){

            int arg_data_count=plotData.Count;
            string arg_data_string="";
            int index=0;
            foreach (Tuple<string,int> part in plotData ) {
                if (index==0) {
                arg_data_string=arg_data_string+part.Item1+":"+part.Item2;
                }
                if (index!=0) {
                arg_data_string=arg_data_string+";"+part.Item1+":"+part.Item2;
                }
                index++;
            }
            //guidelines for arg result
            //string picPath="abc-dedf-gsfh.jpg";
            //example data_string "A:50;B:60;C:70;D:80";
            //data_count of above string : "4";
            //charter's shell command is python .\chartLib.py test.jpg "A:50;B:60;C:70;D:80" 4
            string arg = $"{picPath} {arg_data_string} {arg_data_count}";
            return arg;
        }

        
    }
}