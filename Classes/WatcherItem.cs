using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Watcher.Classes
{
    public class WatcherItem : INotifyPropertyChanged
    {
        private string _name;
        private string _host;
        private string _type_check;
        private string _raw;
        private string[][] _headers;
        private string _data;
        private bool _status;
        private bool _is_critical;
        private int _time_check;

        private string[] _down_times = new string[0];
        private string[] _up_times = new string[0];

        public string Title
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        public string TypeCheck
        {
            get { return _type_check; }
            set { _type_check = value; }
        }
        public string[][] Headers
        {
            get { return _headers; }
            set { _headers = value; }
        }
        public string RAW
        {
            get { return _raw; }
            set { _raw = value; }
        }

        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public bool Status
        {
            get { return _status; }
            set { if (_status != value) { _status = value; NotifyPropertyChanged("Status"); }; }
        }

        public bool IsCritical
        {
            get { return _is_critical; }
            set { _is_critical = value; }
        }

        public int TimeCheck
        {
            get { return _time_check; }
            set { _time_check = value; }
        }

        public string[] DownTimes
        {
            get { return _down_times; }
            set { _down_times = value; }
        }

        public string[] UpTimes
        {
            get { return _up_times; }
            set { _up_times = value; }
        }

        public WatcherItem(string _name, string _host, string _type_check, string _raw, string _data, string[][] _headers, bool _is_critical, int _time_check)
        {

            this._name = _name;
            this._host = _host;
            this._type_check = _type_check;
            this._headers = _headers;
            this._data = _data;
            this._status = false;
            this._is_critical = _is_critical;
            this._time_check = _time_check;
            this._raw = _raw;



        }

        void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
