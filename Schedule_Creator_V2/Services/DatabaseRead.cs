using Microsoft.Data.SqlClient;
using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Enums;
using Schedule_Creator_V2.Models.Records;

namespace Schedule_Creator_V2.Services
{
    internal class DatabaseRead : Database
    {

        public static List<ScheduleRow> GetSchedule()
        {
            List<ScheduleRow> returnList = new List<ScheduleRow>();

            using (var reader = ExecuteReader(
                """
                SELECT
                *
                FROM
                [UWGB].[Schedule]
                """
                ))
            while (reader.Read())
            {
                returnList.Add(new ScheduleRow(
                    Enum.Parse<DayOfWeek>((string)reader["dayOfWeek"]),
                    (int)reader["staffID"]
                    ));
            }
            return returnList;
        }

        public static List<ScheduleRow> GetScheduleOnDay(DayOfWeek day)
        {
            List<ScheduleRow> returnList = new List<ScheduleRow>();

            using (var reader = ExecuteReader(
                """
                SELECT
                *
                FROM
                [UWGB].[Schedule]
                WHERE
                dayOfWeek == @dayOfWeek
                """,
                new SqlParameter("@dayOfWeek", day)
                ))
                while (reader.Read())
                {
                    returnList.Add(new ScheduleRow(
                        Enum.Parse<DayOfWeek>((string)reader["dayOfWeek"]),
                        (int)reader["staffID"]
                        ));
                }
            return returnList;
        }

        public static List<JobSettings> ReadJobSettings()
        {
            List<JobSettings> returnList = new List<JobSettings>();

            using (var reader = ExecuteReader(
                """
                SELECT
                    *
                FROM
                    [UWGB].[JobSettings]
                """
                ))
            while (reader.Read())
                {
                    returnList.Add(new JobSettings(
                        Enum.Parse<DayOfWeek>((string)reader["DayOfWeek"]),
                        TimeOnly.FromTimeSpan((TimeSpan)reader["OpeningTime"]),
                        TimeOnly.FromTimeSpan((TimeSpan)reader["ClosingTime"])
                        ));
                }
            return returnList;
        }
        public static List<Staff> ReadStaffAvailOnDay(DayOfWeek dayOfTheWeek)
        {
            List<Staff> returnList = new List<Staff>();
            using (var reader = ExecuteReader("""
                                SELECT *
                FROM [UWGB].[Staff]
                WHERE id IN (
                    SELECT id
                    FROM [UWGB].[Availability]
                    WHERE dayOfTheWeek = @dayOfTheWeek
                );
                """,
                new SqlParameter("@dayOfTheWeek", dayOfTheWeek)
                ))
            {
                while (reader.Read())
                {
                    returnList.Add(new Staff(
                        (int)reader["id"],
                        (string)reader["fName"],
                        (string)reader["mName"],
                        (string)reader["lName"],
                        Enum.Parse<Positions>((string)reader["position"]),
                        (string)reader["email"],
                        bool.Parse((string)reader["belayCert"]),
                        reader["certifiedOn"] is DBNull ? null : DateOnly.FromDateTime((DateTime)reader["certifiedOn"]),
                        reader["expiresOn"] is DBNull ? null : DateOnly.FromDateTime((DateTime)reader["expiresOn"])
                    ));
                }
            }
            return returnList;
        }
        public static List<Staff> ReadStaffWithNoAvail()
        {
            List<Staff> returnList = new List<Staff>();
            using (var reader = ExecuteReader("""
                                SELECT *
                FROM [UWGB].[Staff]
                WHERE id NOT IN (
                    SELECT id
                    FROM [UWGB].[Availability]
                );
                """))
            {
                while (reader.Read())
                {
                    returnList.Add(new Staff(
                        (int)reader["id"],
                        (string)reader["fName"],
                        (string)reader["mName"],
                        (string)reader["lName"],
                        Enum.Parse<Positions>((string)reader["position"]),
                        (string)reader["email"],
                        bool.Parse((string)reader["belayCert"]),
                        reader["certifiedOn"] is DBNull ? null : DateOnly.FromDateTime((DateTime)reader["certifiedOn"]),
                        reader["expiresOn"] is DBNull ? null : DateOnly.FromDateTime((DateTime)reader["expiresOn"])
                    ));
                }
            }
            return returnList;
        }
        public static List<Staff> ReadStaff()
        {
            List<Staff> returnList = new List<Staff>();
            using (var reader = ExecuteReader("SELECT * FROM [UWGB].[Staff]"))
            {
                while (reader.Read())
                {
                    returnList.Add(new Staff(
                        (int)reader["id"],
                        (string)reader["fName"],
                        (string)reader["mName"],
                        (string)reader["lName"],
                        Enum.Parse<Positions>((string)reader["position"]),
                        (string)reader["email"],
                        bool.Parse((string)reader["belayCert"]),
                        reader["certifiedOn"] is DBNull ? null : DateOnly.FromDateTime((DateTime)reader["certifiedOn"]),
                        reader["expiresOn"] is DBNull ? null : DateOnly.FromDateTime((DateTime)reader["expiresOn"])
                    ));
                }
            }
            return returnList;
        }

        public static Staff ReadStaffByID(int id)
        {
            using (var reader = ExecuteReader("SELECT * FROM [UWGB].[Staff] WHERE id = @id", new SqlParameter("@id", id)))
            {
                if (reader.Read())
                {
                    return new Staff(
                        (int)reader["id"],
                        (string)reader["fName"],
                        (string)reader["mName"],
                        (string)reader["lName"],
                        Enum.Parse<Positions>((string)reader["position"]),
                        (string)reader["email"],
                        bool.Parse((string)reader["belayCert"]),
                        reader["certifiedOn"] is DBNull ? null : DateOnly.FromDateTime((DateTime)reader["certifiedOn"]),
                        reader["expiresOn"] is DBNull ? null : DateOnly.FromDateTime((DateTime)reader["expiresOn"])
                        );
                }
                return new Staff(1, "", "", "", Positions.Unknown, "",false, null, null);
            }
        }



        public static List<Availability> ReadAvailForStaffMem(int id)
        {
            var returnList = new List<Availability>();
            using (var reader = ExecuteReader(
                "SELECT * FROM [UWGB].[Availability] WHERE @id = id",
                new SqlParameter("@id", id)))
            {
                while (reader.Read())
                {
                    returnList.Add(
                        new Availability(
                            id,
                            Enum.Parse<DayOfWeek>(reader["dayOfTheWeek"].ToString()),
                            TimeOnly.FromTimeSpan((TimeSpan)reader["startTime"]),
                            TimeOnly.FromTimeSpan((TimeSpan)reader["endTime"])
                            ));
                }
            }

            return returnList.OrderBy(a => a.dayOfTheWeek).ToList();
        }
     

        public static List<Availability> ReadAvailability()
        {
            List<Availability> returnList = new List<Availability>();
            using (var reader = ExecuteReader("SELECT * FROM [UWGB].[Availability]"))
            {
                while (reader.Read())
                {
                    returnList.Add(new Availability(
                        (int)(reader["id"]),
                        Enum.Parse<DayOfWeek>(reader["dayOfTheWeek"].ToString()),
                        TimeOnly.FromTimeSpan((TimeSpan)reader["startTime"]),
                        TimeOnly.FromTimeSpan((TimeSpan)reader["endTime"])
                        ));
                }
            }
            return returnList;
        }

        public static List<Availability> ReadAvailabilityByID(int id)
        {
            List<Availability> returnList = new List<Availability>();
            using (var reader = ExecuteReader("SELECT * FROM [UWGB].[Availability] WHERE id = @id",
                new SqlParameter("@id", id)
                ))
            {
                while (reader.Read())
                {
                    returnList.Add(new Availability(
                        (int)(reader["id"]),
                        Enum.Parse<DayOfWeek>(reader["dayOfTheWeek"].ToString()),
                        TimeOnly.FromTimeSpan((TimeSpan)reader["startTime"]),
                        TimeOnly.FromTimeSpan((TimeSpan)reader["endTime"])
                        ));
                }
            }
            return returnList;
        }

        public static List<DaysOff> ReadDaysOff()
        {
            List<DaysOff> returnList = new List<DaysOff> ();
            using (var reader = ExecuteReader("SELECT * FROM [UWGB].[DaysOff]"))
            {
                while (reader.Read())
                {
                    returnList.Add(new DaysOff(
                        (int)reader["id"],
                        DateOnly.FromDateTime((DateTime)reader["date"]),
                        (string)reader["reason"]
                        ));
                }
            }
            return returnList;
        }

    }
}
