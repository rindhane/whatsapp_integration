namespace ChartingLib
{
    public class MainProgram
    { 
        
        public static void Main(string[] args ) 
        {
            string exePath=@".\dist\chartLib\chartLib.exe";
            string picPath="abc-dedf-gsfh.jpg";
            //charter's shell command is python .\chartLib.py test.jpg "A:50;B:60;C:70;D:80" 4
            string arg_test_1 = "A:51;B:43;C:55;D:56";
            string arg_test_2 = "4";
            string arg = $"{picPath} {arg_test_1} {arg_test_2}";
            System.Console.WriteLine($"arguments passed are ${arg}");
            pycharter pychart = new pycharter(exePath, arg); 
            pychart.buildChart();
        }
    }
}