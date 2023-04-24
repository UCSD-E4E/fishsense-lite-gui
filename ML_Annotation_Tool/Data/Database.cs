using ML_Annotation_Tool.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_Annotation_Tool.Data
{
    public class Database
    {
        public SQLiteConnection DB_Connection;
        public Database(string directoryPath)
        {
            // platform independent system for embedding a file into the directory
            string DB_SQLiteFileName = Path.Combine(directoryPath, "DB_ImageAnnotations");
 
            string DB_ConnectionString = "Data Source=" + DB_SQLiteFileName;
            DB_Connection = new SQLiteConnection(DB_ConnectionString);
           
            if (!File.Exists(DB_SQLiteFileName))
            {
                SQLiteConnection.CreateFile(DB_SQLiteFileName);
            }
        }

        public IEnumerable<string> RequestAnnotationsForPath(string imagePath)
        {
            CreateTableIfNeeded();
            string query = "SELECT * FROM annotations";
            SQLiteCommand command = new SQLiteCommand(query, DB_Connection);

            OpenConnection();
            SQLiteDataReader result = command.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    if (result["Image Path"].ToString() == imagePath)
                    {
                        yield return result["email"].ToString() + result["id"].ToString();
                    }
                }
            }

        }
        private void CreateTableIfNeeded()
        {
            string CreateCommand = "CREATE TABLE IF NOT EXISTS annotations ('Annotation Key', 'Image Path', 'Top Left X', 'Top Left Y', 'Bottom Right X', 'Button Right Y');";
            SQLiteCommand command = new SQLiteCommand(CreateCommand, DB_Connection);
            OpenConnection();

            int result = command.ExecuteNonQuery();

            CloseConnection();

        }
        private void OpenConnection()
        {
            if (DB_Connection.State != System.Data.ConnectionState.Open)
            {
                DB_Connection.Open();
            }
        }
        private void CloseConnection()
        {
            if (DB_Connection.State != System.Data.ConnectionState.Closed)
            {
                DB_Connection.Close();
            }
        }
    }
}
