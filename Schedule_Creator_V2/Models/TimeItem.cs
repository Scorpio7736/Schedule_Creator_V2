using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_Creator_V2.Models
{
    internal class TimeItem
    {
        public string _display { get; set; }
        public TimeSpan _value { get; set; }

        public TimeItem(string display, TimeSpan value)
        {
            this._display = display;
            this._value = value;
        }

        public List<TimeItem> GetTimeItems()
        {
            var times = new List<TimeItem>();

            DateTime start = DateTime.Today;
            DateTime end = start.AddDays(1);
            TimeSpan interval = TimeSpan.FromMinutes(15);

            for (DateTime t = start; t < end; t = t.Add(interval))
            {
                times.Add(
                    new TimeItem(
                        t.ToString("hh:mm tt"),
                        t.TimeOfDay)
                    );
            }
            return times;
        }
    }
}
