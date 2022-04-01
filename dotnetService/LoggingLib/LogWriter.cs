using System.IO;
using System.Text;

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
            int buffer=4096;
            FileStream fs= new FileStream(_path,
                                        FileMode.Append,
                                        FileAccess.Write,
                                        FileShare.ReadWrite,
                                        buffer, FileOptions.Asynchronous);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);//File.AppendText(_path);
            string timeStamp=System.DateTime.Now.ToString("dd MMM HH:mm:ss");
            note = $"{timeStamp}: " + note;
            sw.WriteLine(note);
            sw.Close();
            sw.Dispose();
        }
    }
}
