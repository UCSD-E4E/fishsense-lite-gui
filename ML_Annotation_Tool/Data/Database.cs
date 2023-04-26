using Avalonia;
using ML_Annotation_Tool.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML_Annotation_Tool.Data
{
    public class Database
    {
        private SQLiteConnection DB_Connection;
        private bool DB_Initialized = false;

        // Is this a good way to do this? The purpose is to initialize the class right afterwards
        public Database() { } 

        public void SetDatabaseConnection(string directoryPath)
        {
            // platform independent system for embedding a file into the directory
            string DB_SQLiteFileName = Path.Combine(directoryPath, "DB_ImageAnnotations.db");

            string DB_ConnectionString = "Data Source=" + DB_SQLiteFileName;
            DB_Connection = new SQLiteConnection(DB_ConnectionString);

            if (!File.Exists(DB_SQLiteFileName))
            {
                SQLiteConnection.CreateFile(DB_SQLiteFileName);
            }
            DB_Initialized = true;
            CreateTableIfNeeded();
        }

        public IEnumerable<string> RequestAnnotationsForPath(string imagePath)
        {
            if (DB_Initialized)
            {
                string query = "SELECT * FROM annotations";
                OpenConnection();
                SQLiteCommand command = new SQLiteCommand(query, DB_Connection);


                SQLiteDataReader result = command.ExecuteReader();

                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        if (result["ImagePath"].ToString() == imagePath)
                        {
                            yield return result["AnnotationDescriptor"].ToString() + result["ImagePath"].ToString() + 
                                result["TopLeftX"].ToString() + result["TopLeftY"].ToString() + 
                                result["BottomRightX"].ToString() + result["BottomRightY"].ToString();
                        }
                    }
                }
                CloseConnection();
            }
        }
        public void InsertData(string annotationDescriptor, string imagePath, Point topLeft, Point bottomRight)
        {
            // insertion command
            string InsertCommand = "INSERT INTO annotations (AnnotationDescriptor, ImagePath, TopLeftX, TopLeftY, " +
                "BottomRightX, BottomRightY) VALUES (@annotationDescriptor, @imagePath, @topLeftX, @topLeftY, " +
                "@bottomRightX, @bottomRightY)";
            OpenConnection();
            SQLiteCommand command = new SQLiteCommand(InsertCommand, DB_Connection);


            // adding parameters to prevent sql injection ?? 
            // https://youtu.be/anTP-mgktiI?t=724
            command.Parameters.AddWithValue("@annotationDescriptor", annotationDescriptor);
            command.Parameters.AddWithValue("@imagePath", imagePath);
            command.Parameters.AddWithValue("@topLeftX", topLeft.X);
            command.Parameters.AddWithValue("@topLeftY", topLeft.Y);
            command.Parameters.AddWithValue("@bottomRightX", bottomRight.X);
            command.Parameters.AddWithValue("@bottomRightY", bottomRight.Y);

            //var z = new ErrorMessageBox(command.);
            int result = command.ExecuteNonQuery();
            CloseConnection();
        }
        private void CreateTableIfNeeded()
        {
            if (DB_Initialized)
            {
                string CreateCommand = "CREATE TABLE IF NOT EXISTS annotations (AnnotationDescriptor varchar(10), ImagePath varchar(200), TopLeftX int, TopLeftY int, " +
                "BottomRightX int, BottomRightY int);";
                OpenConnection();
                SQLiteCommand command = new SQLiteCommand(CreateCommand, DB_Connection);


                int result = command.ExecuteNonQuery();

                CloseConnection();
            }
        }
        private void OpenConnection()
        {
            if (DB_Initialized && DB_Connection.State != System.Data.ConnectionState.Open)
            {
                DB_Connection.Open();
            }
        }
        private void CloseConnection()
        {
            if (DB_Initialized && DB_Connection.State != System.Data.ConnectionState.Closed)
            {
                DB_Connection.Close();
            }
        }
    }
}
