using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Script
{
    public class GSDateTime: GSCore
    {
        private DateTime dateTime = DateTime.Now;

        public override GSCore GetPropertyValue(string s)
        {
            switch (s)
            {
                case "year":
                    return new GSInt64(dateTime.Year);
                case "month":
                    return new GSInt64(dateTime.Month);
                case "day":
                    return new GSInt64(dateTime.Day);
                case "dayOfWeek":
                    return new GSInt64(dayOfWeek);
                case "hour":
                    return new GSInt64(dateTime.Hour);
                case "minute":
                    return new GSInt64(dateTime.Minute);
                case "second":
                    return new GSInt64(dateTime.Second);
                case "millisecond":
                    return new GSInt64(dateTime.Millisecond);
                case "standardDateString":
                    return new GSString(string.Format("{0:0000}{1:00}{2:00}", dateTime.Year, dateTime.Month, dateTime.Day));
                case "standardTimeString":
                    return new GSString(string.Format("{0:00}{1:00}{2:00}", dateTime.Hour, dateTime.Minute, dateTime.Second));

                default:
                    return base.GetPropertyValue(s);
            }
        }

        public override void writeScript(int level, System.IO.StreamWriter sw)
        {
            sw.Write("(datetime {0} {1} {2} {3} {4} {5})", dateTime.Year, dateTime.Month, dateTime.Day,
                dateTime.Hour, dateTime.Minute, dateTime.Second);
        }
            private int dayOfWeek
            {
                get
                {
                    switch(dateTime.DayOfWeek)
                    {
                        case DayOfWeek.Sunday:
                            return 0;
                        case DayOfWeek.Monday:
                            return 1;
                        case DayOfWeek.Tuesday:
                            return 2;
                        case DayOfWeek.Wednesday:
                            return 3;
                        case DayOfWeek.Thursday:
                            return 4;
                        case DayOfWeek.Friday:
                            return 5;
                        case DayOfWeek.Saturday:
                            return 6;
                        default:
                            return 0;
                    }
                }
            }
    }
}
