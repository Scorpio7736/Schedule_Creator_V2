using Microsoft.Data.SqlClient;
using Schedule_Creator_V2.Models.Enums;

namespace Schedule_Creator_V2.Services
{
    internal class DatabaseUpdate : Database
    {

        public static void UpdateJobSettings(DayOfWeek day, TimeOnly openingTime, TimeOnly closingTime)
        {
            ExecuteNonQuery(
                "UPDATE [UWGB].[JobSettings] SET OpeningTime = @openingTime, ClosingTime = @closingTime WHERE DayOfWeek = @day",
                new SqlParameter("@day", day.ToString()),
                new SqlParameter("@openingTime", openingTime),
                new SqlParameter("@closingTime", closingTime)
                );
        }
        public static void UpdateStaff(int id, string fName, string mName, string lName, Positions position, string studentEmail)
        {
            ExecuteNonQuery(
                "UPDATE [UWGB].[Staff] SET fName = @fName, mName = @mName, lName = @lName, position = @position, email = @email WHERE id = @id" ,
                new SqlParameter("@id", id),
                new SqlParameter("@fName", fName),
                new SqlParameter("@mName", mName),
                new SqlParameter("@lName", lName),
                new SqlParameter("@position", position.ToString()),
                new SqlParameter("@email", studentEmail)
                );
        }

        public static void UpdateBelayCert(int id, bool isBelayCertified, DateOnly? certifiedOn = null, DateOnly? expiresOn = null)
        {
            string commandText = @"
        UPDATE [UWGB].[Staff]
        SET belayCert = @belayCert,
            certifiedOn = @certifiedOn,
            expiresOn = @expiresOn
        WHERE id = @id";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@id", id),
                new SqlParameter("@belayCert", isBelayCertified.ToString()),

                new SqlParameter(
                    "@certifiedOn",
                    certifiedOn.HasValue ? (object)certifiedOn.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value
                    ),

                new SqlParameter(
                    "@expiresOn",
                    expiresOn.HasValue ? (object)expiresOn.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value
                    )
            };

            ExecuteNonQuery(commandText, parameters);
        }

    }
}
