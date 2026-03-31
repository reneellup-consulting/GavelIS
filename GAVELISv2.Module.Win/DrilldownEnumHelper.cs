using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GAVELISv2.Module.Win
{
    public static class DrilldownEnumHelper
    {
        public static int GetPaymentMode(string p)
        {
            string f = p.Substring(4);
            switch (f)
            {
                case "Check":
                    return 0;
                case "Cash":
                    return 1;
                case "Wire":
                    return 2;
                case "Others":
                    return 3;
                default:
                    return 0;
            }
        }

        public static int GetPaymentTypeNo(string p)
        {
            switch (p)
            {
                case "Check":
                    return 0;
                case "Cash":
                    return 1;
                case "Wire":
                    return 2;
                case "Others":
                    return 3;
                default:
                    return 0;
            }
        }

        public static int GetMonthMode(string m)
        {
            string f = m.Substring(0, 3);
            switch (f)
            {
                case "Jan":
                    return 1;
                case "Feb":
                    return 2;
                case "Mar":
                    return 3;
                case "Apr":
                    return 4;
                case "May":
                    return 5;
                case "Jun":
                    return 6;
                case "Jul":
                    return 7;
                case "Aug":
                    return 8;
                case "Sep":
                    return 9;
                case "Oct":
                    return 10;
                case "Nov":
                    return 11;
                case "Dec":
                    return 12;
                default:
                    return 0;
            }
        }

        public static int GetMonthNo(string p)
        {
            switch (p)
            {
                case "January":
                    return 1;
                case "February":
                    return 2;
                case "March":
                    return 3;
                case "April":
                    return 4;
                case "May":
                    return 5;
                case "June":
                    return 6;
                case "July":
                    return 7;
                case "August":
                    return 8;
                case "September":
                    return 9;
                case "October":
                    return 10;
                case "November":
                    return 11;
                case "December":
                    return 12;
                default:
                    return 0;
            }
        }
    }
}
