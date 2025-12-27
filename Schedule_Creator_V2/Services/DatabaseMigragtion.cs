using Microsoft.Data.SqlClient;
using Schedule_Creator_V2.Services;
using System.IO;

namespace Schedule_Creator_V2
{
    internal class DatabaseMigragtion : Database
    {

        public static void Migration()
        {
            DataFolderExists();
        }

        private static void DataFolderExists()
        {
            string dataPath = Path.Combine(AppContext.BaseDirectory, "Data");

            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            AppDomain.CurrentDomain.SetData("DataDirectory", dataPath);
        }

    }
}
