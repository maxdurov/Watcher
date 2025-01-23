using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using Watcher.Classes;
using Watcher.Components;


namespace Watcher.Pages
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Page, INotifyPropertyChanged
    {

        private ObservableCollection<WatcherItem> _list_of_watchers = new ObservableCollection<WatcherItem>();

        public ObservableCollection<WatcherItem> List => _list_of_watchers;

        MainWindow context;



        public Main(MainWindow context)
        {
            InitializeComponent();
            this.context = context;
            Notifications.OnListUpdated += ((List<WatcherItem> list) => { Application.Current.Dispatcher.Invoke(() => _InitCollectionOfWatchers()); });
            EventsHandler.NewEvent += ((obj) => { NewEventNotif(true); });
            context.Engine.Stats.StatisticsUpdated += UpdateStats;
            context.Engine.Stats.TimeUpdated += UpdateTime;
            var view = CollectionViewSource.GetDefaultView(_list_of_watchers);
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription("Status", ListSortDirection.Ascending));
            _InitCollectionOfWatchers();
            context.Engine.Start();
            Version.Content = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.DataContext = this;
        }

        private void UpdateTime(string time)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.StatsTime.Text = time;
            });

        }

        private void NewEventNotif(bool IsShow)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!ToEvents.IsEnabled) return;
                if (IsShow)
                {
                    ToEventsNotif.Visibility = Visibility.Visible;
                }
                else
                {
                    ToEventsNotif.Visibility = Visibility.Hidden;
                }
            });


        }

        private void UpdateStats(int online, int offline, int all, int percent, SolidColorBrush status)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                StatsOnline.Text = online.ToString();
                StatsOffline.Text = offline.ToString();
                StatsAll.Text = all.ToString();
                progressCircle.Progress = percent;
                StatusNotif.Background = status;

            });
        }

        private void _InitCollectionOfWatchers()
        {
            var list = context.Engine.getList();

            _list_of_watchers.Clear();
            foreach (WatcherItem item in list)
            {
                _list_of_watchers.Add(item);
            }
            CollectionViewSource.GetDefaultView(_list_of_watchers).Refresh();
            NotifyPropertyChanged();
        }

        //void ProgressTest()
        //{
        //    Task.Run(() =>
        //    {
        //        try
        //        {
        //            for (int i = 0; i <= 100; i++)
        //            {

        //                Thread.Sleep(10);
        //                progressCircle.Dispatcher.Invoke(() =>
        //                {
        //                    progressCircle.Progress = i;

        //                });

        //                if (i == 100)
        //                {
        //                    i = 0;

        //                    foreach (var item in _list_of_watchers)
        //                    {
        //                        if (item.Status)
        //                        {
        //                            item.Status = false;                                
        //                        }
        //                        else
        //                        {
        //                            item.Status = true;
        //                        }
        //                    }


        //                }

        //            }
        //        }
        //        catch
        //        {

        //        }
        //    });

        //}


        void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ToSettings_Click(object sender, RoutedEventArgs e)
        {
            context.MainFrame.Navigate(new Settings.Settings(context));

        }

        private void ToEvents_Click(object sender, RoutedEventArgs e)
        {
            NewEventNotif(false);
            ToEvents.IsEnabled = false;
            Events window = new Events(context);
            window.Show();
            window.Closed += ((obj, arg) => { Application.Current.Dispatcher.Invoke(() => { ToEvents.IsEnabled = true; }); });
        }
    }
}
