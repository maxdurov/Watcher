using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;

namespace Watcher.Components
{
    /// <summary>
    /// Interaction logic for CircleProgressBar.xaml
    /// </summary>
    public partial class CircleProgressBar : UserControl
    {

        public static readonly DependencyProperty ProgressProperty =
        DependencyProperty.Register("Progress", typeof(double), typeof(CircleProgressBar), new PropertyMetadata(default(double), null, OnChangeProgressProperty));


        public CircleProgressBar()
        {
            InitializeComponent();
            
            //this.DataContext = this;
            Stroke = new SolidColorBrush(Colors.Black);
        }

        public Brush Stroke {
            get
            {
               return progressPath.Stroke;
            }
            set
            {
                progressPath.Stroke = value;
            }
            }

        private double progress = 0.00;
        public double Progress { 
            get {
                return (double)GetValue(ProgressProperty);
            } 
            set { 
                SetValue(ProgressProperty, value);
            } 
        }

        private static object OnChangeProgressProperty(DependencyObject d, object baseValue)
        {
            var val = (double)baseValue;
            var control = d as CircleProgressBar;
            
            if(control != null)
            {
                control.AnimateProgress(val);
            }

            return baseValue;
            
        }

        private void AnimateProgress(double percentage)
        {
            if (percentage < 0 || percentage > 100) return;

            double angle = 360 * ((percentage-1) / 100);
            bool isLargeArc = angle > 180;

            
            progressArc.Point = CalculateArcPoint(angle);
            progressArc.IsLargeArc = isLargeArc;
            progressText.Content = percentage + "%";
        }

        private Point CalculateArcPoint(double angle)
        {
            double radius = 85; // Радиус дуги
            double radians = Math.PI * angle / 180;
            double x = 100 + radius * Math.Sin(radians);
            double y = 100 - radius * Math.Cos(radians);
            return new Point(x, y);
        }

    }
}
