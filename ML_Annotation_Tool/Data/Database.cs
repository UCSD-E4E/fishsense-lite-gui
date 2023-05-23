using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace FishSenseLiteGUI.Data
{
    /// <summary>
    /// Purpose: Class that interacts directly with the SQLite Database. This class will create a table
    ///          if one has not been created, and read/write data from the database.
    /// Notes: Each annotation is stored in the following format as a row in the Annotations table: 
    /// 
    ///           | Annotation Key | Image Path | Top Left X | Top Left Y | Bottom Right X | Bottom Right Y |
    /// 
    ///        Please note that the last 4 columns are pixel values relating to the image data itself, NOT
    ///        the location as chosen on the display. These are entirely different data points and the database 
    ///        itself has no knowledge of the screen at all (in order to ensure it is compliant with all screens
    ///        the user may display the app from.
    /// </summary>
    public class Database
    {
        private SQLiteConnection DatabaseConnection;

        public Database(string directoryPath)
        {
            // Path.Combine is operating system independent, and directoryPath is routed in from the FileExplorerCommand
            string DatabaseSQLiteFileName = Path.Combine(directoryPath, "DB_ImageAnnotations.db");

            string DatabaseConnectionString = "Data Source=" + DatabaseSQLiteFileName;
            DatabaseConnection = new SQLiteConnection(DatabaseConnectionString);

            // Users can annotate new data, or build off of previous annotations.
            if (!File.Exists(DatabaseSQLiteFileName))
            {
                SQLiteConnection.CreateFile(DatabaseSQLiteFileName);
            }
            CreateTableIfNeeded();
        } 
        
        /// <summary>
        /// Purpose: Returns annotation data stored in the database regarding an individual filename (imagePath)
        /// </summary>
        public IEnumerable<string[]> RequestAnnotationsForPath(string imagePath)
        {
            string query = "SELECT AnnotationDescriptor, ImagePath, TopLeftX, TopLeftY, BottomRightX, BottomRightY FROM annotations";

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

                        yield return data;
                    }
                }
            }
            // Closing the connection is quick, and ensures no data loss.
            CloseConnection();
        }
        
        /// <summary>
        /// Purpose: Inserts data directly into the SQLite database, specifically when the user wants to add an 
        ///          annotation/draws a valid box on the GUI. This can only be called from the DatabaseModel class,
        ///          as no other object has direct access to this class.
        /// </summary>
        public async void InsertData(string annotationDescriptor, string imagePath, int topLeftX, int topLeftY, int bottomRightX, int bottomRightY)
        {
            // Insertion command. The @ signs are placeholders for individual datapoints.
            string InsertCommand = "INSERT INTO annotations (AnnotationDescriptor, ImagePath, TopLeftX, TopLeftY, " +
                "BottomRightX, BottomRightY) VALUES (@annotationDescriptor, @imagePath, @topLeftX, @topLeftY, " +
                "@bottomRightX, @bottomRightY)";
            OpenConnection();
            SQLiteCommand command = new SQLiteCommand(InsertCommand, DatabaseConnection);

            // Adding Parameters in this format protects against basic SQL injection tactics.
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
            if (DatabaseConnection.State != System.Data.ConnectionState.Open)
            {
                await DatabaseConnection.OpenAsync();
            }
        }
        private async void CloseConnection()
        {
            if (DatabaseConnection.State != System.Data.ConnectionState.Closed)
            {
                await DatabaseConnection.CloseAsync();
            }
        }
    }
}
