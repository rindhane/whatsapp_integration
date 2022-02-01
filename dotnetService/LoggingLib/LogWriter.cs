using System.IO;

namespace LoggingLib { 

    public interface IlogWriter{
        public void writeNotification(string note);
    }
    public class logWriter:IlogWriter {
        
        private string _path ;
        public logWriter() {
            _path = System.Environment.GetEnvironmentVariable("logFile");
        }

        public void writeNotification(string note ) {
            string timeStamp=System.DateTime.Now.ToString("dd MMM HH:mm:ss");
            note = $"{timeStamp}: " + note;
            StreamWriter sw = File.AppendText(_path);
            sw.WriteLine(note);
            sw.Close();
            sw.Dispose();
        }
    }
}
