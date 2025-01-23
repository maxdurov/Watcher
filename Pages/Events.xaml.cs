using System;
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
using System.Windows.Shapes;
using Watcher.Classes;

namespace Watcher.Pages
{
    /// <summary>
    /// Interaction logic for Events.xaml
    /// </summary>
    public partial class Events : Window
    {
        MainWindow mw;
        public Events(MainWindow mw)
        {
            this.mw = mw;
            EventsHandler.NewEvent += NewEvent;
            InitializeComponent();

            TextBlock.Text = mw.Engine.EventsHandler.GetEvents();

            Focus();

        }

        private void ScrollHandler()
        {
            double a = (TextBlock.ActualHeight + TextBlock.VerticalOffset);
            double b = (TextBlock.ExtentHeight - 50);

            if (a >= b)
            {
                TextBlock.ScrollToEnd();
            }
        }

        private void NewEvent(string text)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                TextBlock.AppendText(text);
                ScrollHandler();
            });
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }

        private void ToClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
