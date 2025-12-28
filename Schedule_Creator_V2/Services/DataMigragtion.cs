using Microsoft.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace Schedule_Creator_V2.Services
{
    internal class DataMigragtion : Database
    {
        public static void EnsureDatabaseExists()
        {
            var databaseExists = DatabaseExists();
            if (databaseExists) return;
            CreateDatabase();
        }

        private static bool DatabaseExists()
        {
            var databaseFileName = GetDatabaseFileName();
            var databasePath = Path.GetDirectoryName(databaseFileName);
            var directoryExists = Directory.Exists(databasePath);
            if (!directoryExists) Directory.CreateDirectory(databasePath);
            return File.Exists(databaseFileName);
        }

        private static void CreateDatabase()
        {
            CreateDatabaseFile();
            CreateDatabaseSchema();
        }


        private static void CreateDatabaseFile()
        {
            var databaseFileName = GetDatabaseFileName();
            var logFileName = databaseFileName.Replace(".mdf", "_log.ldf");

            //Need to create special connection on this one command only because it is creating the database itself
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalDbConnectionMaster"].ConnectionString))
            {
                connection.Open();
                var createDbSql = Queries.CreateDatabase
                    .Replace("{databaseFileName}", databaseFileName)
                    .Replace("{logFileName}", logFileName);
                using (var cmd = new SqlCommand(createDbSql, connection))
                {
                    var result = cmd.ExecuteNonQuery();
                }
            }

        }

        private static void CreateDatabaseSchema()
        {
            ExecuteNonQuery(Queries.CreateSchemas);
            ExecuteNonQuery(Queries.CreateTables);
        }

        private static string GetDatabaseFileName()
        {
            var localApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbfile = Path.Combine(
                localApplicationDataPath,
                nameof(Schedule_Creator_V2),
                $"AppData.mdf");
            return dbfile;
        }

    }
}
