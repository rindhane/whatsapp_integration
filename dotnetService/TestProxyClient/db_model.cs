using Microsoft.Data.Sqlite;
using System.IO;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using ProxyClient;
using Microsoft.Extensions.Configuration;

namespace dbSetup{

    class DbConnection:IDbModel {
        public SqliteConnection connection ;
        public string DbString;

        public DbConnection(IWebHostEnvironment webHostEnvironment, IConfiguration Configuration) {
            DbString=Path.Combine(webHostEnvironment.ContentRootPath,
                        Configuration["DBNAME"]);
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

        public object[] GetUserIdRegistration(int userID) {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = 
            @"Select * 
            FROM REGISTRATION
            WHERE ID=$userID
            ";
            command.Parameters.AddWithValue ("$userID",userID);
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
        public object[] GetRegistrationDetails(string userPhone) {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = 
            @"Select * 
            FROM REGISTRATION
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
        public void DeleteRegistration(long userID)
        {
            connection.Open();
            var transaction = connection.BeginTransaction();
            var command=connection.CreateCommand();
            command.CommandTimeout = 60;
            command.CommandText=
            @"DELETE FROM REGISTRATION
            where ID=$value1 
            ";
            command.Parameters.AddWithValue("$value1", userID);
            command.ExecuteNonQuery();
            transaction.Commit();
            connection.Close();
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

        public object[] GetUserDetails(long userID) {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = 
            @"Select * 
            FROM users
            WHERE ID=$value1
            ";
            command.Parameters.AddWithValue ("$value1",userID);
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

        public void AddUser(object[] userDetails)
        {
            connection.Open();
            var transaction = connection.BeginTransaction();
            var command=connection.CreateCommand();
            command.CommandTimeout = 60;
            command.CommandText=
            @"INSERT INTO USERS
            VALUES ($ID, $UserName, $Phone, $UserGroup) 
            ";
            command.Parameters.AddWithValue("$ID", $"{(long)userDetails[0]}");
            command.Parameters.AddWithValue("$UserName", $"{(string)userDetails[1]}");
            command.Parameters.AddWithValue("$Phone",$"{(string)userDetails[2]}");
            command.Parameters.AddWithValue("$UserGroup", $"{(string)userDetails[3]}");
            command.ExecuteNonQuery();
            transaction.Commit();
            connection.Close();
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

        //to record the sent message from the client
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
        //record of inocming message
        public void messageReceived(UserMessageContainer response, string DialogueID) {
            string formatString= "dd-MM-yyyy HH:mm:ss";
            string KIND= "Incoming";
            string ReadStatus = "Received";
            string IncomingText = !String.IsNullOrEmpty(response.ButtonText) ? response.ButtonText : response.Body;
            connection.Open();
            var transaction = connection.BeginTransaction();
            var command=connection.CreateCommand();
            command.CommandTimeout = 60;
            command.CommandText=
            @"INSERT INTO MessageStatus
            VALUES ($value1, $value2, $value3, $value4, $value5, $value6) 
            ";
            Console.WriteLine(response.MessageSid+" : responseId");
            command.Parameters.AddWithValue("$value1", $"{response.MessageSid}");
            command.Parameters.AddWithValue("$value2", $"{DialogueID}"); 
            command.Parameters.AddWithValue("$value3",$"{DateTime.Now.ToString(formatString)}");
            command.Parameters.AddWithValue("$value4", $"{KIND}"); 
            command.Parameters.AddWithValue("$value5",$"{IncomingText}"); 
            command.Parameters.AddWithValue("$value6",$"{ReadStatus}");
            command.ExecuteNonQuery();
            transaction.Commit();
            connection.Close();
            return ;           
        }

        public void updateStatus(Status status){
            connection.Open();
            var transaction = connection.BeginTransaction();
            var command=connection.CreateCommand();
            command.CommandTimeout = 60;
            command.CommandText=
            @"UPDATE MessageStatus
            SET ReadStatus = $value1
            WHERE MessageID= $value2 
            ";
            command.Parameters.AddWithValue("$value1", $"{status.MessageStatus}");
            command.Parameters.AddWithValue("$value2", $"{status.MessageSid}");
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
        public List<Tuple<long,string>> GetUserInProductionGroup(string group ) {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = 
            @"Select * 
            FROM ProductionGroup
            WHERE ProdGroup=$group
            ";
            command.Parameters.AddWithValue("$group",group);
            var reader=command.ExecuteReader();
            List<long> users =new List<long>();
            while (reader.Read())
            {
            users.Add(reader.GetInt64(1));//storing the ids
            }
            reader.Close();
            connection.Close();
            List<Tuple<long,string>> result = new List<Tuple<long,string>> (); 
            foreach(long userID in users)
            {
                object[] temp = GetUserDetails(userID);
                result.Add(new Tuple<long,string>((long)temp[0],(string)temp[2]));
            }
            return result;
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

        public void updateEscalationStatus(string deviceID, string status, long escalationState)
        {
            connection.Open();
            var transaction = connection.BeginTransaction();
            var command=connection.CreateCommand();
            command.CommandText=
            @"UPDATE Escalation
            SET LAST_STATUS=$value1 , EscalationState=$value2 
            WHERE ID=$value3
            ";
            command.Parameters.AddWithValue("$value1", status);
            command.Parameters.AddWithValue("$value2", escalationState);
            command.Parameters.AddWithValue("$value3", deviceID);
            command.ExecuteNonQuery();
            transaction.Commit();
            connection.Close();
        }
        public void insertEscalation(string deviceID, string status, long escalationState){
            string formatString= DateTimeHelpers.dateStringFormat();
            connection.Open();
            var transaction = connection.BeginTransaction();
            var command=connection.CreateCommand();
            command.CommandTimeout = 60;
            command.CommandText=
            @"INSERT INTO Escalation 
            VALUES ($ID, $TIME_RECORD, $LAST_STATUS, $STATE ) 
            ";
            command.Parameters.AddWithValue("$ID", $"{deviceID}");
            command.Parameters.AddWithValue("$TIME_RECORD", $"{DateTime.Now.ToString(formatString)}");
            command.Parameters.AddWithValue("$LAST_STATUS",$"{status}");
            command.Parameters.AddWithValue("$STATE",$"{escalationState}");
            command.ExecuteNonQuery();
            transaction.Commit();
            connection.Close();
        }
        public void deleteEscalation(string deviceID)
        {
            connection.Open();
            var transaction = connection.BeginTransaction();
            var command=connection.CreateCommand();
            command.CommandTimeout = 60;
            command.CommandText=
            @"DELETE FROM Escalation
            where ID=$value1 
            ";
            command.Parameters.AddWithValue("$value1", deviceID);
            command.ExecuteNonQuery();
            transaction.Commit();
            connection.Close();

        } 

        public void updateEscalation(string deviceID, string status, int type, long escalationState)
        {
            if (type==-1)
            {
                deleteEscalation(deviceID);
                return;
            }
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = 
            @"Select * 
            FROM Escalation
            WHERE ID=$deviceID
            ";
            command.Parameters.AddWithValue("$deviceID",deviceID);
            var reader=command.ExecuteReader();
            if (reader.Read())
            {
                reader.Close();
                connection.Close();
                updateEscalationStatus(deviceID, status, escalationState);
                return ;
            }
            reader.Close();
            connection.Close();
            insertEscalation(deviceID,status,escalationState);
            return ;
        }

        public List<Tuple<long,string>> GetUserAlertGroup(string group ) {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = 
            @"Select * 
            FROM users
            WHERE UserGroup=$group
            ";
            command.Parameters.AddWithValue("$group",group);
            var reader=command.ExecuteReader();
            List<Tuple<long,string>> result =new List<Tuple<long,string>>();
            while (reader.Read())
            {
            var temp= new Tuple<long,String>(reader.GetInt64(0),reader.GetString(2));
            result.Add(temp);
            }
            reader.Close();
            connection.Close();
            return result;

        }

        public List<object[]> GetPendingNotifications(){
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = 
            @"Select * 
            FROM Escalation
            ";
            //command.Parameters.AddWithValue("$value",EscalationType);
            var reader=command.ExecuteReader();
            //List<Tuple<string,string>> result =new List<Tuple<string,string>>();
            List<object[]> result = new List<object[]>();
            while (reader.Read())
            {
                object[] temp  = new object[4];
                reader.GetValues(temp);
                //var temp= new Tuple<string,string>(reader.GetString(1),reader.GetString(2)); //passing tuple of (date,last_status)
                result.Add(temp);
            }
            reader.Close();
            connection.Close();
            return result;
        }

    }
}