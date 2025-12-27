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

        private static bool DataFolderExists()
        {
            DirectoryInfo? current = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            while (current != null && !current.EnumerateFiles("*.sln").Any())
            {
                current = current.Parent;
            }

            if (current == null)
            {
                return false;
            }

            string dataPath = Path.Combine(current.FullName, "Data");
            return Directory.Exists(dataPath);
        }

    }
}
