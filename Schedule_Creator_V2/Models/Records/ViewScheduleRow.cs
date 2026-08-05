namespace Schedule_Creator_V2.Models
{
    internal class ViewScheduleRow
    {
        public string AvailMon { get; set; } = string.Empty;
        public TimeOnly? MonStartTime { get; set; }
        public TimeOnly? MonEndTime { get; set; }

        public string AvailTue { get; set; } = string.Empty;
        public TimeOnly? TueStartTime { get; set; }
        public TimeOnly? TueEndTime { get; set; }

        public string AvailWed { get; set; } = string.Empty;
        public TimeOnly? WedStartTime { get; set; }
        public TimeOnly? WedEndTime { get; set; }

        public string AvailThu { get; set; } = string.Empty;
        public TimeOnly? ThuStartTime { get; set; }
        public TimeOnly? ThuEndTime { get; set; }

        public string AvailFri { get; set; } = string.Empty;
        public TimeOnly? FriStartTime { get; set; }
        public TimeOnly? FriEndTime { get; set; }

        public string AvailSat { get; set; } = string.Empty;
        public TimeOnly? SatStartTime { get; set; }
        public TimeOnly? SatEndTime { get; set; }

        public string AvailSun { get; set; } = string.Empty;
        public TimeOnly? SunStartTime { get; set; }
        public TimeOnly? SunEndTime { get; set; }
    }
}