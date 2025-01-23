using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watcher.Classes
{
    public class Notifications
    {
        public delegate void ConfigUpdatedHandler();

        public static event ConfigUpdatedHandler? OnConfigUpdated;

        public delegate void ListUpdatedHandler(List<WatcherItem> list);

        public static event ListUpdatedHandler? OnListUpdated;

        public static void NotifyConfig()
        {
            OnConfigUpdated?.Invoke();
        }

        public static void NotifyListUpdated(List<WatcherItem> list)
        {
            OnListUpdated?.Invoke(list);
        }
    }
}
