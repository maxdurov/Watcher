using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Watcher.Classes
{
    public class DataManager : Notifications
    {
        private static readonly string _fileconf = "configuration.json";
        public static bool CheckExistFile()
        {
            return File.Exists(_fileconf);

        }

        public static bool ResetConfError()
        {
            MessageBoxResult dialog = MessageBox.Show("Хотите сбросить файл конфигурации?", "Сброс конфигурации", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dialog == MessageBoxResult.Yes)
            {
                if (CreateConfFile(true))
                {
                    NotifyConfig();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
        public static bool CreateConfFile(bool rewrite = false)
        {
            if (CheckExistFile())
            {
                if (!rewrite)
                {
                    return false;
                }
            }

            FileStream fs = File.Create(_fileconf);

            JObject settings = new JObject();

            settings.Add("typeRating", "ONLY_CRITICAL");
            settings.Add("Items", new JArray());

            string settings_txt = JsonConvert.SerializeObject(settings, Formatting.Indented);

            fs.Write(Encoding.UTF8.GetBytes(settings_txt), 0, Encoding.UTF8.GetBytes(settings_txt).Length);

            fs.Dispose();
            return true;
        }

        public static JObject GetConfFile()
        {
            if (!CheckExistFile())
            {
                if (!CreateConfFile())
                {
                    return null;
                }
            }
            JObject conf = null;

        Start:

            try
            {
                conf = JObject.Parse(File.ReadAllText(_fileconf));
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка чтения файла конфигурации:\n" + e.Message);
                if (ResetConfError())
                {
                    goto Start;
                }
                else
                {
                    Environment.Exit(0);
                }
            }

            return conf;

        }

        public static bool WriteToConf(JObject conf)
        {
            if (!CheckExistFile())
            {
                if (!CreateConfFile())
                {
                    return false;
                }
            }

            using (var fs = File.Open(_fileconf, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Write(Encoding.UTF8.GetBytes(conf.ToString()), 0, Encoding.UTF8.GetBytes(conf.ToString()).Length);
            }
            return true;


        }
        public static (bool result, string reason) AddItem(WatcherItem item)
        {
            if (!CheckExistFile())
            {
                if (!CreateConfFile())
                {
                    return (false, "Ошибка создания конфигурационного файла");
                }
            }

            JObject conf = GetConfFile();


            foreach (var _item in conf["Items"])
            {
                WatcherItem wi = JsonConvert.DeserializeObject<WatcherItem>(_item.ToString());
                if (wi.Title == item.Title)
                {
                    return (false, "Такое название уже используется. Выберите другое");
                }
            }


            ((JArray)conf["Items"]).Add(JsonConvert.SerializeObject(item));

            bool ret = WriteToConf(conf);
            NotifyConfig();
            return (ret, "Не удалось сохранить изменения");
        }

        public static bool EditItem(WatcherItem item, string title)
        {
            JObject conf = GetConfFile();
            JArray arr = (JArray)conf["Items"];
            for (int i = 0; i < arr.Count; i++)
            {
                WatcherItem wi = JsonConvert.DeserializeObject<WatcherItem>(arr[i].ToString());
                if (wi.Title.Equals(title))
                {
                    WatcherItem watcherItem = wi;
                    watcherItem = item;
                    arr[i] = JsonConvert.SerializeObject(item);
                    conf["Items"] = arr;
                    bool ret = WriteToConf(conf);
                    NotifyConfig();
                    return ret;
                }
            }
            return false;

        }

        public static bool DeleteItem(WatcherItem item)
        {
            JObject conf = GetConfFile();
            JArray arr = (JArray)conf["Items"];
            for (int i = 0; i < arr.Count; i++)
            {
                WatcherItem wi = JsonConvert.DeserializeObject<WatcherItem>(arr[i].ToString());
                if (wi.Title.Equals(item.Title))
                {
                    arr.RemoveAt(i);
                    bool ret = WriteToConf(conf);
                    NotifyConfig();
                    return ret;
                }
            }
            return false;
        }

        public static List<WatcherItem> GetItems()
        {
            List<WatcherItem> items = new List<WatcherItem>();
            if (!CheckExistFile())
            {
                return items;
            }

            JObject conf = GetConfFile();
            foreach (string item in (JArray)conf["Items"])
            {
                items.Add(JsonConvert.DeserializeObject<WatcherItem>(item));
            }

            return items;
        }

        public static string GetStatusSetting()
        {
            var conf = GetConfFile();

            return ((string)conf["typeRating"]);
        }

        public static void SetStatusSetting(string param)
        {
            var conf = GetConfFile();

            conf["typeRating"] = param;

            WriteToConf(conf);
            NotifyConfig();
        }

    }
}
