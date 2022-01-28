using Microsoft.Data.Sqlite;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using WhatsappService;
namespace dbSetup{

    public class MessageRecord{
        public string MessageID {get;set;}
        public string DialogueID {get;set;}
        public string TIME_RECORD {get;set;}
        public string KIND {get;set;}
        public string MessageText {get;set;}
        public string ReadStatus {get;set;}

        public override string ToString()
        {
            return base.ToString()+"||"+"messageID:"+ MessageID 
                                    + '|'+"DialogueID:" + DialogueID
                                    + '|'+ "TIME_RECORD:" + TIME_RECORD 
                                    + '|'+ "KIND:"+ KIND
                                    + '|'+ "MessageText:"+ MessageText
                                    + '|'+ "ReadStatus:" + ReadStatus;
        }

    }
    class DbConnection:IDbModel {
        public SqliteConnection connection ;
        public string DbString;

        public DbConnection(IWebHostEnvironment webHostEnvironment) {
            DbString=Path.Combine(webHostEnvironment.ContentRootPath,
                        System.Environment.GetEnvironmentVariable("DBNAME"));
            connection=new SqliteConnection($"Data Source={DbString}");
        }
        
        public bool IsUser(string userPhone) {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = 
            @"Select * 
            FROM users
            WHERE Phone=$phone
            ";
            command.Parameters.AddWithValue ("$phone",userPhone);
            var reader=command.ExecuteReader();
            if (reader.Read())
            {
            object[] arr = new object[4];
            reader.GetValues(arr);
            //System.Console.WriteLine($"arr: {arr[0]}");
            reader.Close();
            connection.Close();
            return true; 
            }
            reader.Close();
            connection.Close();
            return false;
        }
        public object[] GetUserDetails(string userPhone) {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = 
            @"Select * 
            FROM users
            WHERE Phone=$phone
            ";
            command.Parameters.AddWithValue ("$phone",userPhone);
            var reader=command.ExecuteReader();
            if (reader.Read())
            {
            object[] arr = new object[4];
            reader.GetValues(arr);
            reader.Close();
            connection.Close();
            return arr; 
            }
            reader.Close();
            connection.Close();
            return null;
        }
        public object[] GetSession(long userId) {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = 
            @"Select * 
            FROM SessionStatus
            WHERE UserID=$id
            ";
            command.Parameters.AddWithValue ("$id", userId);
            var reader= command.ExecuteReader();
            if(reader.Read()) {
                object[] arr = new object[5];
                reader.GetValues(arr);
                reader.Close();
                connection.Close();
                return arr;
            }
            reader.Close();
            connection.Close();
            return null;
        }

        public void messageSent(MessageRecord record) {
            connection.Open();
            var transaction = connection.BeginTransaction();
            var command=connection.CreateCommand();
            command.CommandTimeout = 60;
            command.CommandText=
            @"INSERT INTO MessageStatus
            VALUES ($value1, $value2, $value3, $value4, $value5, $value6) 
            ";
            command.Parameters.AddWithValue("$value1", $"{record.MessageID}");
            command.Parameters.AddWithValue("$value2", $"{record.DialogueID}");
            command.Parameters.AddWithValue("$value3",$"{record.TIME_RECORD}");
            command.Parameters.AddWithValue("$value4", $"{record.KIND}");
            command.Parameters.AddWithValue("$value5",$"{record.MessageText}");
            command.Parameters.AddWithValue("$value6",$"{record.ReadStatus}");
            command.ExecuteNonQuery();
            transaction.Commit();
            connection.Close();           
        }
        
        public void generateSession(MessageRecord record, long userId ,int category , int stage) {
            connection.Open();
            var transaction = connection.BeginTransaction();
            var command=connection.CreateCommand();
            command.CommandTimeout = 60;
            command.CommandText=
            @"INSERT INTO SessionStatus
            VALUES ($value1 , $value2, $value3 , $value4 , $value5 ) 
            ";
            command.Parameters.AddWithValue("$value1", $"{DialogFlow.GenerateSessionId().ToString()}");
            command.Parameters.AddWithValue("$value2", $"{userId}");
            command.Parameters.AddWithValue("$value3",$"{category}");
            command.Parameters.AddWithValue("$value4", $"{stage}");
            command.Parameters.AddWithValue("$value5",$"{record.DialogueID}");
            command.ExecuteNonQuery();
            transaction.Commit();
            connection.Close(); 
        }
        public void updateSession(MessageRecord record, object[] parentSession, int nextStage ) {
            connection.Open();
            var transaction = connection.BeginTransaction();
            var command=connection.CreateCommand();
            command.CommandTimeout = 60;
            command.CommandText=
            @"UPDATE SessionStatus
            set SessionID=$value1, Stage=$value2, DialogueID=$value3
            where SessionID=$value4 
            ";
            command.Parameters.AddWithValue("$value1", $"{DialogFlow.GenerateSessionId().ToString()}");
            command.Parameters.AddWithValue("$value2", nextStage);
            command.Parameters.AddWithValue("$value3",record.DialogueID );
            command.Parameters.AddWithValue("$value4", (string) parentSession[0]);
            command.ExecuteNonQuery();
            transaction.Commit();
            connection.Close();
        }
        public void clearSession(string parentSessionId ) {
            connection.Open();
            var transaction = connection.BeginTransaction();
            var command=connection.CreateCommand();
            command.CommandTimeout = 60;
            command.CommandText=
            @"DELETE FROM SessionStatus
            where SessionID=$value1 
            ";
            command.Parameters.AddWithValue("$value1", parentSessionId);
            command.ExecuteNonQuery();
            transaction.Commit();
            connection.Close();
        } 

    }
}