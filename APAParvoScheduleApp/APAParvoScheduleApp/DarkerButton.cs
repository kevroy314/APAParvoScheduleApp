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
    public class DarkerButton : Button
    {
        private double m_darkOpacity = 0.6;

        public double DarkOpacity
        {
            get { return m_darkOpacity; }
            set { m_darkOpacity = value; }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Border border = GetTemplateChild("Background") as Border;
            Rectangle rect = GetTemplateChild("BackgroundGradient") as Rectangle;

            if (border != null)
            {
                border.Background = this.Background;
                border.Opacity = m_darkOpacity;
            }
            if (rect != null)
            {
                LinearGradientBrush lbrush = rect.Fill as LinearGradientBrush;
                if (lbrush != null)
                {
                    lbrush.Opacity = m_darkOpacity;
                }
            }
        }
    }
}
