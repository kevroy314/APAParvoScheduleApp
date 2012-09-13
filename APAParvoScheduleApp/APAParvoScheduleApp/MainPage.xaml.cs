using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace APAParvoScheduleApp
{
    public partial class MainPage : UserControl
    {
        private SolidColorBrush selectStrokeBrush;
        private SolidColorBrush unselectStrokeBrush;
        private Point mouseDownGridPoint;
        private List<Tuple<int, int, int[]>> schedule;
        
        public MainPage()
        {
            InitializeComponent();
            for (int i = 0; i < scheduleNeedsGrid.Children.Count; i++)
            {
                ((Rectangle)scheduleNeedsGrid.Children[i]).MouseLeftButtonDown += new MouseButtonEventHandler(scheduleNeedsGrid_MouseLeftButtonDown);
                ((Rectangle)scheduleNeedsGrid.Children[i]).MouseLeftButtonUp += new MouseButtonEventHandler(scheduleNeedsGrid_MouseLeftButtonUp);
                ((Rectangle)scheduleNeedsGrid.Children[i]).MouseEnter += new MouseEventHandler(scheduleNeedsGrid_MouseEnter);
                ((Rectangle)scheduleNeedsGrid.Children[i]).MouseLeave += new MouseEventHandler(scheduleNeedsGrid_MouseLeave);
            }
            selectStrokeBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            unselectStrokeBrush = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
            calendar.DisplayDateChanged += new EventHandler<CalendarDateChangedEventArgs>(calendar_DisplayDateChanged);
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void scheduleNeedsGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            scheduleGridPopup.IsOpen = false;
        }

        void scheduleNeedsGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            scheduleGridPopup.HorizontalOffset = mousePos.X;
            scheduleGridPopup.VerticalOffset = mousePos.Y;
            scheduleGridPopup.IsOpen = true;
        }

        void scheduleNeedsGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int row = (int)((Rectangle)sender).GetValue(Grid.RowProperty);
            int col = (int)((Rectangle)sender).GetValue(Grid.ColumnProperty);
            if (mouseDownGridPoint.X == row && mouseDownGridPoint.Y == col)
            {
                DateTime selectedDate = gridIndexToDate(col + row * 7);
                if (selectedDate.Month != calendar.DisplayDate.Month)
                    calendar.DisplayDate = selectedDate;
                else
                {
                    calendar.SelectedDate = selectedDate;
                    calendar.SelectedDates.Clear();
                    calendar.SelectedDates.Add(selectedDate);
                }
            }
        }

        void scheduleNeedsGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int row = (int)((Rectangle)sender).GetValue(Grid.RowProperty);
            int col = (int)((Rectangle)sender).GetValue(Grid.ColumnProperty);
            mouseDownGridPoint = new Point(row, col);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            loadMonthData(new int[] { 1, 2, 3, 4, 5, 6, 7, 
                                      4, 5, 6, 2, 1, 0, 1, 
                                      3, 0, 9, 3, 0, 0, 0,
                                      0, 0, 0, 0, 0, 0, 0, 
                                      0, 0}, new DateTime(calendar.DisplayDate.Year, calendar.DisplayDate.Month, 1));
        }

        void calendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            for (int i = 0; i < scheduleNeedsGrid.Children.Count; i++)
                ((Rectangle)scheduleNeedsGrid.Children[i]).Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                Calendar senderCalendar = (Calendar)sender;
                DateTime? selectedDate = (DateTime)e.AddedItems[0];
                SolidColorBrush currentBrush = (SolidColorBrush)((Rectangle)scheduleNeedsGrid.Children[dateToGridIndex(selectedDate.Value)]).Fill;
                SolidColorBrush newBrush = new SolidColorBrush(Color.FromArgb(255, currentBrush.Color.R, currentBrush.Color.G, currentBrush.Color.B));
                ((Rectangle)scheduleNeedsGrid.Children[dateToGridIndex(selectedDate.Value)]).Fill = newBrush;
                ((Rectangle)scheduleNeedsGrid.Children[dateToGridIndex(selectedDate.Value)]).Stroke = selectStrokeBrush;
            }
            if (e.RemovedItems.Count > 0)
            {
                Calendar senderCalendar = (Calendar)sender;
                DateTime? selectedDate = (DateTime)e.RemovedItems[0];
                SolidColorBrush currentBrush = (SolidColorBrush)((Rectangle)scheduleNeedsGrid.Children[dateToGridIndex(selectedDate.Value)]).Fill;
                SolidColorBrush newBrush = new SolidColorBrush(Color.FromArgb(128, currentBrush.Color.R, currentBrush.Color.G, currentBrush.Color.B));
                ((Rectangle)scheduleNeedsGrid.Children[dateToGridIndex(selectedDate.Value)]).Fill = newBrush;
                ((Rectangle)scheduleNeedsGrid.Children[dateToGridIndex(selectedDate.Value)]).Stroke = unselectStrokeBrush;
            }
        }

        private DateTime gridIndexToDate(int gridIndex)
        {
            DateTime firstDateOfMonth = new DateTime(calendar.DisplayDate.Year, calendar.DisplayDate.Month, 1);
            int addDays = gridIndex - (int)firstDateOfMonth.DayOfWeek;
            if (firstDateOfMonth.DayOfWeek == DayOfWeek.Sunday)
                addDays -= 7;
            return firstDateOfMonth.AddDays(addDays);
        }

        private int dateToGridIndex(DateTime date)
        {
            DateTime firstDateOfMonth = new DateTime(date.Year, date.Month, 1);
            double firstDayOffset = 8 - (double)firstDateOfMonth.DayOfWeek;
            int col = (int)date.DayOfWeek;
            int row = (int)Math.Floor(((double)date.Day - firstDayOffset) / 7.0) + 2;
            if (firstDateOfMonth.DayOfWeek == DayOfWeek.Sunday)
                row++;
            return col + row * 7;
        }

        private void loadMonthData(int[] dayValues, DateTime startDate)
        {
            for (int i = 0; i < dayValues.Length; i++)
                ((Rectangle)scheduleNeedsGrid.Children[dateToGridIndex(startDate.AddDays(i))]).Fill = new SolidColorBrush(getHeatMapColor(dayValues[i], 0, 5));
        }

        private Color getHeatMapColor(double v, double vmin, double vmax)
        {
            double r = 1.0f;
            double g = 1.0f;
            double b = 1.0f;

            double dv;

            if (v < vmin)
                v = vmin;
            if (v > vmax)
                v = vmax;
            dv = vmax - vmin;

            if (v < (vmin + 0.25 * dv))
            {
                r = 0;
                g = 4 * (v - vmin) / dv;
            }
            else if (v < (vmin + 0.5 * dv))
            {
                r = 0;
                b = 1 + 4 * (vmin + 0.25 * dv - v) / dv;
            }
            else if (v < (vmin + 0.75 * dv))
            {
                r = 4 * (v - vmin - 0.5 * dv) / dv;
                b = 0;
            }
            else
            {
                g = 1 + 4 * (vmin + 0.75 * dv - v) / dv;
                b = 0;
            }

            byte rbyte = (byte)(255f * r);
            byte gbyte = (byte)(255f * g);
            byte bbyte = (byte)(255f * b);

            return Color.FromArgb(128, rbyte, gbyte, bbyte);
        }
    }
}
