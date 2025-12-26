using System.Configuration;
using Microsoft.Data.SqlClient;

namespace Schedule_Creator_V2.Services
{
    internal class Database
    {
        /// <summary>
        /// Executes a non query command in the database. Primary use is going to be for INSERT INTO statements.
        /// </summary>
        /// <param name="commandText">SQL Command Text</param>
        /// <param name="getParameters">Perameters</param>
        /// <returns>Null</returns>
        public static void ExecuteNonQuery(string commandText, params SqlParameter[] getParameters)
        {
            using (var sqlCommand = GetNewSqlCommand())
            {
                sqlCommand.CommandText = commandText;
                sqlCommand.CommandType = System.Data.CommandType.Text;
                foreach (var parameter in getParameters)
                {
                    sqlCommand.Parameters.Add(parameter);
                }
                sqlCommand.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Executes a Reader command. Primary use will be SELECT x FROM y statements.
        /// </summary>
        /// <param name="commandText">SQL Command Text</param>
        /// <param name="addParameters">Perameters</param>
        /// <returns>A Reader Command to the Database</returns>
        public static SqlDataReader ExecuteReader(string commandText, params SqlParameter[] getParameters)
        {
            var sqlCommand = GetNewSqlCommand();
            sqlCommand.CommandText = commandText;
            sqlCommand.CommandType = System.Data.CommandType.Text;
            foreach (var parameter in getParameters)
            {
                sqlCommand.Parameters.Add(parameter);
            }
            return sqlCommand.ExecuteReader();
        }
        /// <summary>
        /// Executes a Scalar command in the database. Primary use is for getting IDs. It returns first column of the first row affected.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">SQL Command Text</param>
        /// <param name="addParameters">Nullable Perameter arguments</param>
        /// <returns>
        /// An execute Command
        /// </returns>
        public static T ExecuteScalar<T>(string commandText, params SqlParameter[] getParameters)
        {
            using (var sqlCommand = GetNewSqlCommand())
            {
                sqlCommand.CommandText = commandText;
                sqlCommand.CommandType = System.Data.CommandType.Text;
                foreach (var parameter in getParameters)
                {
                    sqlCommand.Parameters.Add(parameter);
                }
                var result = sqlCommand.ExecuteScalar();
                return (T)result;
            }
        }

        /// <summary>
        /// Creates the start of a SQL Command for use in later functions.
        /// </summary>
        /// <returns>
        /// None
        /// </returns>
        private static SqlCommand GetNewSqlCommand()
        {
            SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["LocalDbConnection"].ConnectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            return sqlCommand;
        }
    }
}