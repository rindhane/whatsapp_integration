using Microsoft.Data.Sqlite;
using System;
namespace dbSetup{
    class DbConnection {
        public SqliteConnection connection ;

        public DbConnection(string path) {
            connection=new SqliteConnection($"Data Source={path}");
        }
        public void writeStatement (string statement) {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = 
            @"Select * 
            FROM users
            WHERE id=$id
            ";
            command.Parameters.AddWithValue ("$id",10);
            var reader= command.ExecuteReader();
            while (reader.Read()) {
                Console.WriteLine("it ran");
                var result= reader.GetString(1);
                Console.WriteLine(result);
            }
            reader.Close();
            connection.Close();
        }

    }
}