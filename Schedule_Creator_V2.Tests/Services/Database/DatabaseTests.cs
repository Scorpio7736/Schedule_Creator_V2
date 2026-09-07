using Microsoft.Data.SqlClient;
using System.Data;

using DatabaseBase =
    Schedule_Creator_V2.Services.Database.Database;

namespace Schedule_Creator_V2.Tests.Services.Database
{
    public sealed class DatabaseTests :
        IClassFixture<DatabaseTestFixture>
    {
        public DatabaseTests(
            DatabaseTestFixture fixture)
        {
            /*
             * The fixture creates the isolated LocalDB database
             * and its test table before these tests execute.
             */
        }


        // =========================================================
        // EXECUTE NON QUERY
        // =========================================================

        [Fact]
        public void ExecuteNonQuery_ValidInsert_InsertsRow()
        {
            // Arrange

            Guid testGroup =
                Guid.NewGuid();

            const string textValue =
                "ExecuteNonQuery Test";

            const int numberValue =
                42;

            const string insertSql =
                """
                INSERT INTO dbo.DatabaseTests
                (
                    TestGroup,
                    TextValue,
                    NumberValue
                )
                VALUES
                (
                    @testGroup,
                    @textValue,
                    @numberValue
                );
                """;


            // Act

            DatabaseBase.ExecuteNonQuery(
                insertSql,

                new SqlParameter(
                    "@testGroup",
                    SqlDbType.UniqueIdentifier)
                {
                    Value =
                        testGroup
                },

                new SqlParameter(
                    "@textValue",
                    SqlDbType.NVarChar,
                    100)
                {
                    Value =
                        textValue
                },

                new SqlParameter(
                    "@numberValue",
                    SqlDbType.Int)
                {
                    Value =
                        numberValue
                });


            // Assert

            const string countSql =
                """
                SELECT COUNT(*)
                FROM dbo.DatabaseTests
                WHERE TestGroup = @testGroup;
                """;

            int result =
                DatabaseBase.ExecuteScalar<int>(
                    countSql,

                    new SqlParameter(
                        "@testGroup",
                        SqlDbType.UniqueIdentifier)
                    {
                        Value =
                            testGroup
                    });

            Assert.Equal(
                1,
                result);
        }


        // =========================================================
        // EXECUTE NON QUERY WITH PARAMETERS
        // =========================================================

        [Fact]
        public void ExecuteNonQuery_WithParameters_SavesCorrectValues()
        {
            // Arrange

            Guid testGroup =
                Guid.NewGuid();

            const string expectedText =
                "Parameterized Insert";

            const int expectedNumber =
                125;

            const string insertSql =
                """
                INSERT INTO dbo.DatabaseTests
                (
                    TestGroup,
                    TextValue,
                    NumberValue
                )
                VALUES
                (
                    @testGroup,
                    @textValue,
                    @numberValue
                );
                """;


            // Act

            DatabaseBase.ExecuteNonQuery(
                insertSql,

                new SqlParameter(
                    "@testGroup",
                    testGroup),

                new SqlParameter(
                    "@textValue",
                    expectedText),

                new SqlParameter(
                    "@numberValue",
                    expectedNumber));


            // Assert

            const string selectSql =
                """
                SELECT
                    TextValue,
                    NumberValue
                FROM dbo.DatabaseTests
                WHERE TestGroup = @testGroup;
                """;

            using SqlDataReader reader =
                DatabaseBase.ExecuteReader(
                    selectSql,

                    new SqlParameter(
                        "@testGroup",
                        testGroup));

            Assert.True(
                reader.Read());

            Assert.Equal(
                expectedText,
                reader.GetString(
                    reader.GetOrdinal(
                        "TextValue")));

            Assert.Equal(
                expectedNumber,
                reader.GetInt32(
                    reader.GetOrdinal(
                        "NumberValue")));
        }


        // =========================================================
        // EXECUTE SCALAR
        // =========================================================

        [Fact]
        public void ExecuteScalar_IntegerResult_ReturnsValue()
        {
            // Arrange

            const string commandText =
                """
                SELECT CAST(42 AS INT);
                """;


            // Act

            int result =
                DatabaseBase.ExecuteScalar<int>(
                    commandText);


            // Assert

            Assert.Equal(
                42,
                result);
        }


        [Fact]
        public void ExecuteScalar_StringResult_ReturnsValue()
        {
            // Arrange

            const string commandText =
                """
                SELECT CAST(
                    'Hello World'
                    AS NVARCHAR(100)
                );
                """;


            // Act

            string result =
                DatabaseBase.ExecuteScalar<string>(
                    commandText);


            // Assert

            Assert.Equal(
                "Hello World",
                result);
        }


        [Fact]
        public void ExecuteScalar_WithParameter_UsesParameterValue()
        {
            // Arrange

            const string commandText =
                """
                SELECT @numberValue;
                """;

            const int expected =
                73;


            // Act

            int result =
                DatabaseBase.ExecuteScalar<int>(
                    commandText,

                    new SqlParameter(
                        "@numberValue",
                        SqlDbType.Int)
                    {
                        Value =
                            expected
                    });


            // Assert

            Assert.Equal(
                expected,
                result);
        }


        // =========================================================
        // EXECUTE READER
        // =========================================================

        [Fact]
        public void ExecuteReader_ExistingRows_ReturnsRows()
        {
            // Arrange

            Guid testGroup =
                Guid.NewGuid();

            InsertTestRow(
                testGroup,
                "First",
                10);

            InsertTestRow(
                testGroup,
                "Second",
                20);

            const string commandText =
                """
                SELECT
                    TextValue,
                    NumberValue
                FROM dbo.DatabaseTests
                WHERE TestGroup = @testGroup
                ORDER BY NumberValue;
                """;


            // Act

            using SqlDataReader reader =
                DatabaseBase.ExecuteReader(
                    commandText,

                    new SqlParameter(
                        "@testGroup",
                        testGroup));


            // Assert

            Assert.True(
                reader.Read());

            Assert.Equal(
                "First",
                reader["TextValue"]);

            Assert.Equal(
                10,
                reader["NumberValue"]);

            Assert.True(
                reader.Read());

            Assert.Equal(
                "Second",
                reader["TextValue"]);

            Assert.Equal(
                20,
                reader["NumberValue"]);

            Assert.False(
                reader.Read());
        }


        [Fact]
        public void ExecuteReader_NoMatchingRows_ReturnsEmptyReader()
        {
            // Arrange

            Guid testGroup =
                Guid.NewGuid();

            const string commandText =
                """
                SELECT
                    TextValue,
                    NumberValue
                FROM dbo.DatabaseTests
                WHERE TestGroup = @testGroup;
                """;


            // Act

            using SqlDataReader reader =
                DatabaseBase.ExecuteReader(
                    commandText,

                    new SqlParameter(
                        "@testGroup",
                        testGroup));


            // Assert

            Assert.False(
                reader.Read());
        }


        // =========================================================
        // INVALID SQL
        // =========================================================

        [Fact]
        public void ExecuteNonQuery_InvalidSql_ThrowsSqlException()
        {
            // Arrange

            const string commandText =
                """
                THIS IS NOT VALID SQL;
                """;


            // Act

            Action action =
                () =>
                    DatabaseBase.ExecuteNonQuery(
                        commandText);


            // Assert

            Assert.Throws<SqlException>(
                action);
        }


        [Fact]
        public void ExecuteScalar_InvalidSql_ThrowsSqlException()
        {
            // Arrange

            const string commandText =
                """
                SELECT *
                FROM ThisTableDoesNotExist_UnitTest;
                """;


            // Act

            Action action =
                () =>
                    DatabaseBase.ExecuteScalar<int>(
                        commandText);


            // Assert

            Assert.Throws<SqlException>(
                action);
        }


        // =========================================================
        // HELPERS
        // =========================================================

        private static void InsertTestRow(
            Guid testGroup,
            string textValue,
            int numberValue)
        {
            const string commandText =
                """
                INSERT INTO dbo.DatabaseTests
                (
                    TestGroup,
                    TextValue,
                    NumberValue
                )
                VALUES
                (
                    @testGroup,
                    @textValue,
                    @numberValue
                );
                """;

            DatabaseBase.ExecuteNonQuery(
                commandText,

                new SqlParameter(
                    "@testGroup",
                    testGroup),

                new SqlParameter(
                    "@textValue",
                    textValue),

                new SqlParameter(
                    "@numberValue",
                    numberValue));
        }
    }
}