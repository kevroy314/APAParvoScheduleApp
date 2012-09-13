using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace APAParvoScheduleApp
{
    public class DatabaseInterface
    {
        public static DateTime getStartDay()
        {
            return new DateTime(2012, 09, 17);
        }

        public static int[,] getValueArray(int numDisplayWeeks, int numDaysInWeek)
        {
            int[,] buttonValues = new int[numDisplayWeeks, numDaysInWeek];

            for (int week = 0; week < numDisplayWeeks; week++)
                for (int day = 0; day < numDaysInWeek; day++)
                    buttonValues[week, day] = 0;

            return buttonValues;
        }
        public static bool[,] getLockArray(int numDisplayWeeks, int numDaysInWeek)
        {
            bool[,] lockValues = new bool[numDisplayWeeks, numDaysInWeek];

            for (int week = 0; week < numDisplayWeeks; week++)
                for (int day = 0; day < numDaysInWeek; day++)
                    lockValues[week, day] = false;

            return lockValues;
        }
    }
}
