﻿using System;
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
    public partial class Notifier : ChildWindow
    {
        public Notifier(string title, string content)
        {
            InitializeComponent();
            this.Title = title;
            this.textBlock.Text = content;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

