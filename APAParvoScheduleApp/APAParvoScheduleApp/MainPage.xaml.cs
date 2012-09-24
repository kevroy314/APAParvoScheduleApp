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
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;

namespace APAParvoScheduleApp
{
    public partial class MainPage : UserControl
    {
        private int[,] buttonValues;
        private bool[,] lockValues;

        private const int scaleMax = 7;
        private const int scaleMin = 0;
        private const int numDaysInWeek = 7;

        private DateTime weeksStart;
        private const int numDisplayWeeks = 6;

        private InstructionsWindow instructions;
        private Notifier updateCompleteNotifier;

        public MainPage()
        {
            InitializeComponent();

            instructions = new InstructionsWindow();

            weeksStart = getLastMonday();

            todaysDateLabel.Content = "Todays Date is " + DateTime.Now.ToShortDateString();

            populateWeekList();

            buttonValues = DatabaseInterface.getValueArray(numDisplayWeeks, numDaysInWeek);
            lockValues = DatabaseInterface.getLockArray(numDisplayWeeks, numDaysInWeek);

            updateAllButtonColors(buttonValues);

            updateScheduleButton.Click+=new RoutedEventHandler(updateScheduleButton_Click);
            instructionsButton.Click+=new RoutedEventHandler(instructionsButton_Click);
            weekListComboBox.SelectionChanged+=new SelectionChangedEventHandler(weekListComboBox_SelectionChanged);
        }

        private void populateWeekList()
        {
            for (int i = 0; i < numDisplayWeeks; i++)
                weekListComboBox.Items.Add(weeksStart.AddDays(numDaysInWeek * i).ToShortDateString() + " - " + weeksStart.AddDays(numDaysInWeek * i + numDaysInWeek - 1).ToShortDateString());
            weekListComboBox.SelectedIndex = 0;
        }

        private DateTime getLastMonday()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - (int)DateTime.Now.DayOfWeek + 1);
        }

        private void updateAllButtonColors(int[,] values)
        {
            for (int i = 0; i < scheduleNeedsGrid.Children.Count; i++)
            {
                if (scheduleNeedsGrid.Children[i].GetType() == typeof(DarkerButton))
                {
                    Button currentButton = (Button)scheduleNeedsGrid.Children[i];
                    int column = (int)currentButton.GetValue(Grid.ColumnProperty);
                    if (column >= 0 && column < numDaysInWeek)
                    {
                        currentButton.Background = new SolidColorBrush(getHeatMapColor(values[weekListComboBox.SelectedIndex, i], scaleMin, scaleMax));
                        currentButton.ApplyTemplate();
                    }
                    if (DateTime.Now > weeksStart.AddDays(weekListComboBox.SelectedIndex * numDaysInWeek + i) || lockValues[weekListComboBox.SelectedIndex, i])
                    {
                        ((StackPanel)currentButton.Content).Visibility = System.Windows.Visibility.Visible;
                        currentButton.IsEnabled = false;
                    }
                }
            }
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int column = (int)((Button)sender).GetValue(Grid.ColumnProperty);
            buttonValues[weekListComboBox.SelectedIndex, column]++;
            buttonValues[weekListComboBox.SelectedIndex, column] %= scaleMax;
            updateAllButtonColors(buttonValues);
        }

        private void updateScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            updateCompleteNotifier = new Notifier("Schedule Updated", DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToShortTimeString());
            updateCompleteNotifier.Show();
        }

        private void weekListComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateAllButtonColors(buttonValues);
        }
        
        private void instructionsButton_Click(object sender, RoutedEventArgs e)
        {
            instructions.Show();
        }
    }
}
