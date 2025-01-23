using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Watcher.Classes
{
    public class Statistics
    {

        private int _online = 0;
        private int _offline = 0;
        private int _all = 0;
        private int _percent = 0;
        private string _time = "";
        private bool _critical = false;

        public delegate void StatisticsUpdatedHandler(int online, int offline, int all, int percent, SolidColorBrush? status);
        public event StatisticsUpdatedHandler StatisticsUpdated;

        public delegate void TimeUpdatedHandler(string time);
        public event TimeUpdatedHandler TimeUpdated;


        public Statistics()
        {



            Notifications.OnListUpdated += ((List<WatcherItem> list) => { updateStatistics(list); });

            System.Timers.Timer timer_time = new System.Timers.Timer(1000);
            timer_time.Elapsed += (obj, arg) =>
            {
                TimeUpdater();
            };
            timer_time.AutoReset = true;
            timer_time.Enabled = true;
            timer_time.Start();
        }

        private void TimeUpdater()
        {
            _time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            TimeUpdated?.Invoke(_time);
        }

        private void updateStatistics(List<WatcherItem> list)
        {
            int online = 0;
            int offline = 0;
            bool critical = false;
            foreach (var item in list)
            {
                if (item.Status)
                {
                    online++;
                }
                else
                {
                    offline++;
                    if (item.IsCritical) critical = true;
                }
            }

            _online = online;
            _offline = offline;
            _all = online + offline;
            float a = (float)online / (online + offline) * 100;
            _percent = (int)a;
            _critical = critical;

            StatisticsUpdated?.Invoke(_online, _offline, _all, _percent, Status.getStatus(_percent, _critical));
        }

        void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
