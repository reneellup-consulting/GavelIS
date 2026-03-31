using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GAVELISv2.Module
{
    public static class WorkingDays
    {
        /// <summary> Get working days between two dates (Excluding a list of dates - Holidays) </summary>
        /// <param name="dtmCurrent">Current date time</param>
        /// <param name="dtmFinishDate">Finish date time</param>
        public static int GetWorkingDays(this DateTime dtmCurrent, DateTime dtmFinishDate, List<DayOfWeek> lstExcludedDayOfWeek)
        {
            Func<DateTime, bool> workDay = currentDate =>
                    (
                        //currentDate.DayOfWeek == DayOfWeek.Saturday ||
                        //currentDate.DayOfWeek == DayOfWeek.Sunday ||
                        !lstExcludedDayOfWeek.Exists(evalDay => evalDay.Equals(currentDate.DayOfWeek))
                    );

            return Enumerable.Range(0, 1 + (dtmFinishDate - dtmCurrent).Days).Count(intDay => workDay(dtmCurrent.AddDays(intDay)));
        }
        /// <summary> Get working days between two dates (Excluding a list of dates - Holidays) </summary>
        /// <param name="dtmCurrent">Current date time</param>
        /// <param name="dtmFinishDate">Finish date time</param>
        /// <param name="lstExcludedDates">List of dates to exclude (Holidays)</param>
        public static int GetWorkingDays(this DateTime dtmCurrent, DateTime dtmFinishDate, List<DayOfWeek> lstExcludedDayOfWeek, List<DateTime> lstExcludedDates)
        {
            Func<DateTime, bool> workDay = currentDate =>
                    (
                        //currentDate.DayOfWeek == DayOfWeek.Saturday ||
                        //currentDate.DayOfWeek == DayOfWeek.Sunday ||
                        !lstExcludedDayOfWeek.Exists(evalDay => evalDay.Equals(currentDate.DayOfWeek)) ||
                        !lstExcludedDates.Exists(evalDate => evalDate.Date.Equals(currentDate.Date))
                    );

            return Enumerable.Range(0, 1 + (dtmFinishDate - dtmCurrent).Days).Count(intDay => workDay(dtmCurrent.AddDays(intDay)));
        }
    }
}
