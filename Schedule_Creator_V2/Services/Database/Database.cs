using System.Configuration;
using Microsoft.Data.SqlClient;

namespace Schedule_Creator_V2.Services.Database
{
    internal class Database
    {

        private static string? _connectionStringOverride;


        internal static void SetConnectionString(
            string? connectionString)
        {
            _connectionStringOverride =
                connectionString;
        }


        private static string GetConnectionString()
        {
            if (!string.IsNullOrWhiteSpace(
                    _connectionStringOverride))
            {
                return _connectionStringOverride;
            }

            ConnectionStringSettings? settings =
                ConfigurationManager
                    .ConnectionStrings[
                        "LocalDbConnection"];

            if (settings is null ||
                string.IsNullOrWhiteSpace(
                    settings.ConnectionString))
            {
                throw new InvalidOperationException(
                    "The LocalDbConnection connection string " +
                    "could not be found.");
            }

            return settings.ConnectionString;
        }






        /// <summary>
        /// Executes a command and disposes its connection, including on failure.
        /// </summary>
        public static void ExecuteNonQuery(string commandText, params SqlParameter[] getParameters)
        {
            using var connection = new SqlConnection(GetConnectionString());
            using var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(getParameters);
            connection.Open();
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Returns a reader that owns the open connection.
        /// Callers must close or dispose the reader, even when reading fails.
        /// </summary>
        public static SqlDataReader ExecuteReader(string commandText, params SqlParameter[] getParameters)
        {
            var connection = new SqlConnection(GetConnectionString());
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = commandText;
                command.Parameters.AddRange(getParameters);
                connection.Open();
                return command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            }
            catch
            {
                // No reader was returned to take ownership of this connection.
                connection.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Returns the first value and disposes the connection, including on failure.
        /// </summary>
        public static T ExecuteScalar<T>(string commandText, params SqlParameter[] getParameters)
        {
            using var connection = new SqlConnection(GetConnectionString());
            using var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(getParameters);
            connection.Open();
            return (T)command.ExecuteScalar();
        }
    }
}
