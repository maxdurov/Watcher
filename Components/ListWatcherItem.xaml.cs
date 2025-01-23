using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Watcher.Classes;

namespace Watcher.Components
{
    /// <summary>
    /// Interaction logic for ListWatcherItem.xaml
    /// </summary>


    public partial class ListWatcherItem : UserControl
    {

        public static readonly DependencyProperty IsOnlineProperty =
        DependencyProperty.Register("IsOnline", typeof(bool), typeof(ListWatcherItem), new PropertyMetadata(false, null, OnChangeIsOnline));

        public ListWatcherItem()
        {
            InitializeComponent();

        }

        public static object OnChangeIsOnline(DependencyObject d, object baseValue)
        {
            var control = d as ListWatcherItem;
            if (control != null)
            {
                bool value = (bool)baseValue;
                if (value)
                {
                    control.Border_Item.Background = Classes.Status.GREEN_COLOR;
                    control.Status.Content = "ONLINE";
                }
                else
                {
                    control.Border_Item.Background = Classes.Status.RED_COLOR;
                    control.Status.Content = "OFFLINE";
                }
            }

            return baseValue;
        }

        private static void OnChangeIsOnline(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ListWatcherItem;
            if (control != null)
            {
                if ((bool)e.NewValue)
                {
                    control.Border_Item.Background = new BrushConverter().ConvertFromString("#FF82FFA7") as SolidColorBrush;
                    control.Status.Content = "ONLINE";
                }
                else
                {
                    control.Border_Item.Background = new BrushConverter().ConvertFromString("#FF8282") as SolidColorBrush;
                    control.Status.Content = "OFFLINE";
                }
            }
        }
        public bool IsOnline
        {
            get
            {
                return (bool)GetValue(IsOnlineProperty);
            }
            set
            {
                SetValue(IsOnlineProperty, value);
            }
        }



    }
}
