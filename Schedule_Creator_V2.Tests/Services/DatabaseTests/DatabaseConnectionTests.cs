using Microsoft.Data.SqlClient;
using System.Reflection;
using DatabaseBase = Schedule_Creator_V2.Services.Database.Database;

namespace Schedule_Creator_V2.Tests.Services.Database
{
    // The production helper's connection-string override is process-wide.
    [CollectionDefinition("Database connection lifetime", DisableParallelization = true)]
    public sealed class DatabaseConnectionCollection { }

    [Collection("Database connection lifetime")]
    public sealed class DatabaseConnectionTests
    {
        [Theory]
        [InlineData("nonquery")]
        [InlineData("scalar")]
        [InlineData("reader")]
        [InlineData("reader-close")]
        [InlineData("reader-consumer-error")]
        [InlineData("nonquery-error")]
        [InlineData("scalar-error")]
        [InlineData("reader-error")]
        [InlineData("scalar-cast-error")]
        public void CompletedOperation_ReturnsConnectionToPool(string operation)
        {
            var overrideField = typeof(DatabaseBase).GetField(
                "_connectionStringOverride", BindingFlags.NonPublic | BindingFlags.Static)!;
            string? previousOverride = (string?)overrideField.GetValue(null);
            var settings = new SqlConnectionStringBuilder
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                InitialCatalog = "master",
                IntegratedSecurity = true,
                TrustServerCertificate = true,
                MaxPoolSize = 1,
                ConnectTimeout = 3,
                ApplicationName = "ConnectionLifetime-" + Guid.NewGuid()
            };
            string connectionString = settings.ConnectionString;

            // Warm the pool before applying the short timeout to the regression check.
            var warmSettings = new SqlConnectionStringBuilder(connectionString) { ConnectTimeout = 30 };
            using (var warmConnection = new SqlConnection(warmSettings.ConnectionString))
            {
                warmConnection.Open();
            }

            DatabaseBase.SetConnectionString(connectionString);
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    switch (operation)
                    {
                        case "nonquery":
                            DatabaseBase.ExecuteNonQuery("SELECT 1;");
                            break;
                        case "scalar":
                            Assert.Equal(1, DatabaseBase.ExecuteScalar<int>("SELECT 1;"));
                            break;
                        case "reader":
                            using (var reader = DatabaseBase.ExecuteReader("SELECT 1 UNION ALL SELECT 2;"))
                            {
                                Assert.True(reader.Read());
                                Assert.Equal(1, reader.GetInt32(0));
                                // Dispose before all rows are consumed.
                            }
                            break;
                        case "reader-close":
                            using (var reader = DatabaseBase.ExecuteReader("SELECT 1;"))
                            {
                                reader.Close();
                                Assert.Equal(1, DatabaseBase.ExecuteScalar<int>("SELECT 1;"));
                            }
                            break;
                        case "reader-consumer-error":
                            Assert.Throws<InvalidOperationException>(new Action(() =>
                            {
                                using var reader = DatabaseBase.ExecuteReader("SELECT 1;");
                                Assert.True(reader.Read());
                                throw new InvalidOperationException("Consumer failed.");
                            }));
                            break;
                        case "nonquery-error":
                            Assert.Throws<SqlException>(() =>
                                DatabaseBase.ExecuteNonQuery("THROW 50000, 'Expected test error', 1;"));
                            break;
                        case "scalar-error":
                            Assert.Throws<SqlException>(() =>
                                DatabaseBase.ExecuteScalar<int>("THROW 50000, 'Expected test error', 1;"));
                            break;
                        case "reader-error":
                            Assert.Throws<SqlException>(() =>
                            {
                                using var reader = DatabaseBase.ExecuteReader(
                                    "THROW 50000, 'Expected test error', 1;");
                            });
                            break;
                        case "scalar-cast-error":
                            Assert.Throws<InvalidCastException>(() =>
                                DatabaseBase.ExecuteScalar<int>("SELECT 'not an integer';"));
                            break;
                    }

                    // A leaked connection occupies the only pool slot and makes Open time out.
                    using var probe = new SqlConnection(connectionString);
                    probe.Open();
                }
            }
            finally
            {
                DatabaseBase.SetConnectionString(previousOverride);
                using var poolConnection = new SqlConnection(connectionString);
                SqlConnection.ClearPool(poolConnection);
            }
        }
    }
}
