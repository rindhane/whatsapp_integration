using System.Collections.Generic;
using System;
namespace ChartingLib
{
    public class MainProgram
    { 
        
        public static void Main(string[] args ) 
        {
            //inputs
            string exePath=@".\dist\chartLib\chartLib.exe";
            string picPath="abc-dedf-gsfh.jpg";
                //example data creation
                var data=new List<Tuple<String,int>>();
                data.Add(new Tuple<string,int>("A",51));
                data.Add(new Tuple<string,int>("B",61));
                data.Add(new Tuple<string,int>("C",41));
                data.Add(new Tuple<string,int>("D",48));       
            //preparing commandLine args for the executable
            string arg=pycharter.CreateArgString(picPath,data);
            Console.WriteLine($"arguments passed are ${arg}");
            //executing the charter executable
            pycharter pychart = new pycharter(exePath, arg); 
            pychart.buildChart();
        }
    }
}