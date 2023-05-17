using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace FishSenseLiteGUI.Data
{
    /* Class to interface with Data. This is the data layer.
     * The data is stored in a SQLite database. All the data consists of the annotations.
     * Each annotation is stored as a row in a table. The Data provided are 
     * 
     * | Annotation Key | Image Path | Top Left X | Top Left Y | Bottom Right X | Bottom Right Y |
     * 
     * Each of the X and Y coordinates are relative to the pixels of the image itself, and translated
     * into screen coordinates separately
     */
    public class Database
    {
        private SQLiteConnection DatabaseConnection;

        public Database(string directoryPath)
        {
            /* Because directories path differently in UNIX based systems and Windows, 
             * This uses C#'s built in Path combiner. DirectoryPath is the directory chosen by the 
             * File Explorer Command. */
            string DatabaseSQLiteFileName = Path.Combine(directoryPath, "DB_ImageAnnotations.db");

            string DatabaseConnectionString = "Data Source=" + DatabaseSQLiteFileName;
            DatabaseConnection = new SQLiteConnection(DatabaseConnectionString);

            // This program works for adding annotations to previously annotated data, as well as 
            // creating annotations for previously unannotated data.
            if (!File.Exists(DatabaseSQLiteFileName))
            {
                SQLiteConnection.CreateFile(DatabaseSQLiteFileName);
            }
            CreateTableIfNeeded();
        } 
        
        // RequestAnnotationsForPath returns all the annotations that have a certain path
        // for their Image Path. This is primarily used for drawing the annotation boxes
        // to the images as they are being loaded.
        public IEnumerable<string[]> RequestAnnotationsForPath(string imagePath)
        {
            // This selects all rows from annotations, which is the name of the table
            // The * is a wildcard character that represents the "all rows".
             string query = "SELECT AnnotationDescriptor, ImagePath, TopLeftX, TopLeftY, BottomRightX, BottomRightY FROM annotations";
            
            // Whenever a query is made, a connection is opened, and then closed.
            OpenConnection();

            SQLiteCommand command = new SQLiteCommand(query, DatabaseConnection);
            SQLiteDataReader result = command.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    if (result["ImagePath"].ToString() == imagePath)
                    {
                        string[] data = new string[7];
                        data[0] = result["AnnotationDescriptor"].ToString();
                        data[1] = result["ImagePath"].ToString();
                        data[2] = result["TopLeftX"].ToString();
                        data[3] = result["TopLeftY"].ToString();
                        data[4] = result["BottomRightX"].ToString();
                        data[5] = result["BottomRightY"].ToString();

                        // Yield is similar to its use in Python (and possible other languages).
                        // The individual string array will be returned, and used as a container
                        // for a foreach loop. This allows other parts of the program to just loop
                        // through the function return without separately storing the entire return data.
                        yield return data;
                    }
                }
            }
            // Closing the connection is neessary to ensure no data loss, and other users can access the data
            // at all times.
            CloseConnection();
        }
        
        /* This function is called to insert data directly into the SQLite database, and is generally called 
         * when the user wants to add an annotation/draw the box.
         * The DatabaseModel class will be the only class that calls this method, and provides the method 
         * all the data that will be inserted into the table, allowing this class to have no knowledge of data
         * beyond this class.
         */
        public async void InsertData(string annotationDescriptor, string imagePath, int topLeftX, int topLeftY, int bottomRightX, int bottomRightY)
        {
            // Insertion command. The @ signs are placeholders for individual datapoints.
            string InsertCommand = "INSERT INTO annotations (AnnotationDescriptor, ImagePath, TopLeftX, TopLeftY, " +
                "BottomRightX, BottomRightY) VALUES (@annotationDescriptor, @imagePath, @topLeftX, @topLeftY, " +
                "@bottomRightX, @bottomRightY)";
            OpenConnection();
            SQLiteCommand command = new SQLiteCommand(InsertCommand, DatabaseConnection);

            /* Adding parameters by value is a way to protect against SQL Injection.
            * Directly concatenating strings can be a dangerous task because often users can 
            * insert conditions that query all the data rather than the specific data they should have 
            * access to. The way to protect against it is to ensure all the data will be encapsulated
            * by quotation marks properly, hence the AddWithValue method.
            * https://youtu.be/anTP-mgktiI?t=724
            */
            command.Parameters.AddWithValue("@annotationDescriptor", annotationDescriptor);
            command.Parameters.AddWithValue("@imagePath", imagePath);
            command.Parameters.AddWithValue("@topLeftX", topLeftX);
            command.Parameters.AddWithValue("@topLeftY", topLeftY);
            command.Parameters.AddWithValue("@bottomRightX", bottomRightX);
            command.Parameters.AddWithValue("@bottomRightY", bottomRightY);

            // Executes and this preassumes no return data (hence the NonQuery).
            int result = await command.ExecuteNonQueryAsync();
            CloseConnection();
        }

        /* This program works regardless of whether there are preexisting annotations to the images.
         * If there is no annotations that have been previously written, a table must be created, but otherwise, 
         * command will do nothing.
         */
        private async void CreateTableIfNeeded()
        { 
            string CreateCommand = "CREATE TABLE IF NOT EXISTS annotations (AnnotationDescriptor varchar(10), ImagePath varchar(200)" +
                ", TopLeftX int, TopLeftY int, BottomRightX int, BottomRightY int);";
            OpenConnection();
            SQLiteCommand command = new SQLiteCommand(CreateCommand, DatabaseConnection);

            int result = command.ExecuteNonQuery();

            CloseConnection();
        }
        private async void OpenConnection()
        {
            // Will ensure all connection states will change into Open, not just when the connection is closed.
            if (DatabaseConnection.State != System.Data.ConnectionState.Open)
            {
                await DatabaseConnection.OpenAsync();
            }
        }
        private async void CloseConnection()
        {
            // WIll ensure all connection states will change into Closed, not just when the connection is losed.
            if (DatabaseConnection.State != System.Data.ConnectionState.Closed)
            {
                await DatabaseConnection.CloseAsync();
            }
        }
    }
}
