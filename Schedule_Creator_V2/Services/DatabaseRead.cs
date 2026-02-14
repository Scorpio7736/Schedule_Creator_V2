using Microsoft.Data.SqlClient;
using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Enums;
using Schedule_Creator_V2.Models.Records;

namespace Schedule_Creator_V2.Services
{
    internal class DatabaseRead : Database
    {

        public static List<Staff> ReadStaffWithNoAvail()
        {
            List<Staff> returnList = new List<Staff>();

            using (var reader = ExecuteReader(Queries.ReadStaffWithNoAvail))
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

            return returnList;
        }
        //

        public static List<DayOfWeek> ReadJobSettingsDays()
        {
            List<DayOfWeek> returnList = new List<DayOfWeek>();
            using (var reader = ExecuteReader(Queries.ReadJobSettingsDays))
            while (reader.Read())
            {
                returnList.Add(Enum.Parse<DayOfWeek>((string)reader["DayOfWeek"]));
            }
            return returnList;
        }
        public static List<ScheduleRow> ReadScheduleByScheduleName(string scheduleName)
        {
            List<ScheduleRow> returnList = new List<ScheduleRow>();
            using (var reader = ExecuteReader(
                Queries.ReadScheduleByScheduleName,
                new SqlParameter("@scheduleName", scheduleName)
                ))
                while (reader.Read())
                {
                    returnList.Add(new ScheduleRow(
                        Enum.Parse<DayOfWeek>((string)reader["dayOfWeek"]),
                        (int)reader["staffID"],
                        (string)reader["scheduleName"]
                        ));
                }
            return returnList;
        }

        public static List<string> ReadAllScheduleNames()
        {
                       List<string> returnList = new List<string>();
            using (var reader = ExecuteReader(Queries.ReadAllScheduleNames))
            while (reader.Read())
            {
                returnList.Add((string)reader["scheduleName"]);
            }
            return returnList;
        }

        public static Staff ReadStaffByID(int id)
        {
            using (var reader = ExecuteReader(
                Queries.ReadStaffByID,
                new SqlParameter("@id", id)))
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
                return new Staff(1, "", "", "", Positions.Unknown, "", false, null, null);
            }
        }

        public static List<StaffNameAndAvail> ReadStaffNamesAndAvailOnDay(DayOfWeek day)
        {
            List<StaffNameAndAvail> returnList = new List<StaffNameAndAvail>();

            using (var reader = ExecuteReader(
                Queries.ReadStaffNamesAndAvailOnDay,
                new SqlParameter("@day", day)
                ))
                while (reader.Read())
                {
                    returnList.Add(new StaffNameAndAvail(
                        (int)reader["id"],
                        (string)reader["fName"],
                        (string)reader["lName"],
                        TimeOnly.FromTimeSpan((TimeSpan)reader["startTime"]),
                        TimeOnly.FromTimeSpan((TimeSpan)reader["endTime"])
                        ));
                }
            return returnList;
        }

        public static List<JobSettings> ReadJobSettings()
        {
            List<JobSettings> returnList = new List<JobSettings>();

            using (var reader = ExecuteReader(Queries.ReadJobSettings))
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
            using (var reader = ExecuteReader(
                Queries.ReadStaffAvailOnDay,
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
       
        public static List<Staff> ReadStaff()
        {
            List<Staff> returnList = new List<Staff>();
            using (var reader = ExecuteReader(Queries.ReadStaff))
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

        public static List<Availability> ReadAvailForStaffByID(int id)
        {
            var returnList = new List<Availability>();
            using (var reader = ExecuteReader(
                Queries.ReadAvailForStaffByID,
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

        public static List<Availability> ReadAvailByID(int id)
        {
            List<Availability> returnList = new List<Availability>();
            using (var reader = ExecuteReader(
                Queries.ReadAvailByID,
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
            using (var reader = ExecuteReader(Queries.ReadDaysOff))
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
