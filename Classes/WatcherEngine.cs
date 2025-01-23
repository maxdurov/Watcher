using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using Watcher.Pages;

namespace Watcher.Classes
{
    public class WatcherEngine
    {
        private static List<WatcherItem> items = new List<WatcherItem>();



        private List<System.Timers.Timer> storage = new List<System.Timers.Timer>();

        private bool IsUpdating = false;

        public Statistics Stats = new Statistics();

        public EventsHandler EventsHandler = new EventsHandler();

        public WatcherEngine()
        {


            Notifications.OnConfigUpdated += (() =>
            {
                if (IsUpdating) return;
                Restart();
            });

        }

        public List<WatcherItem> getList()
        {
            return items;
        }

        public void Start()
        {
            items = DataManager.GetItems();
            foreach (var item in items)
            {

                Task.Run(async () =>
                {
                    bool res = await HandlerWatcher(item);
                    item.Status = res;
                    if (res)
                    {
                        EventsHandler.AddEvent("[" + item.Title + "][" + item.Host + "]\t" + " СОЕДИНЕНИЕ УСТАНОВЛЕНО");
                    }
                    else
                    {
                        EventsHandler.AddEvent("[" + item.Title + "][" + item.Host + "]\t" + " СОЕДИНЕНИЕ НЕ УСТАНОВЛЕНО");
                    }
                    Debug.WriteLine(res.ToString() + " " + item.Title);
                    Notifications.NotifyListUpdated(WatcherEngine.items);
                });


                System.Timers.Timer timer = new System.Timers.Timer(item.TimeCheck);
                timer.Elapsed += (obj, arg) =>
                {
                    Task.Run(async () =>
                    {
                        bool res = await HandlerWatcher(item);
                        if (res != item.Status)
                        {
                            item.Status = res;
                            if (res)
                            {
                                EventsHandler.AddEvent("[" + item.Title + "][" + item.Host + "]\t" + " СОЕДИНЕНИЕ УСТАНОВЛЕНО");
                            }
                            else
                            {
                                EventsHandler.AddEvent("[" + item.Title + "][" + item.Host + "]\t" + " СОЕДИНЕНИЕ ПОТЕРЯНО");
                            }
                            Notifications.NotifyListUpdated(WatcherEngine.items);
                        }

                        Debug.WriteLine(res.ToString() + " " + item.Title);
                    });

                };
                timer.AutoReset = true;
                timer.Enabled = true;


                storage.Add(timer);
            }
            Notifications.NotifyListUpdated(items);

        }

        public void Stop()
        {

            foreach (var item in storage)
            {
                item.Enabled = false;
                item.Dispose();
            }
            storage.Clear();

        }

        async public void Restart()
        {
            if (IsUpdating) return;
            await Task.Run(() => { Stop(); });
            Start();

        }

        async private Task<bool> HandlerWatcher(WatcherItem watcherItem)
        {
            switch (watcherItem.TypeCheck)
            {
                case "GET (CODE)":
                    {
                        var req = await getRawResponse(watcherItem.TypeCheck, watcherItem.Host, watcherItem.TimeCheck, watcherItem.Headers);

                        if (req.err != null)
                        {
                            return false;
                        }

                        if ((int)req.resp.StatusCode == int.Parse(watcherItem.RAW))
                        {
                            req.resp?.Dispose();
                            return true;
                        }
                        else
                        {
                            req.resp?.Dispose();
                            return false;
                        }
                    }
                case "GET (BODY)":
                    {
                        var req = await getRawResponse(watcherItem.TypeCheck, watcherItem.Host, watcherItem.TimeCheck, watcherItem.Headers);

                        if (req.err != null)
                        {
                            return false;
                        }

                        string resp = await req.resp.Content.ReadAsStringAsync();
                        if (resp.Equals(watcherItem.RAW))
                        {
                            req.resp?.Dispose();
                            return true;
                        }
                        else
                        {
                            req.resp?.Dispose();
                            return false;
                        }
                    }
                case "POST (CODE)":
                    {
                        StringContent sc = null;
                        if (watcherItem.Data != null) sc = new StringContent(watcherItem.Data);
                        var req = await getRawResponse(watcherItem.TypeCheck, watcherItem.Host, watcherItem.TimeCheck, watcherItem.Headers, sc);

                        if (req.err != null)
                        {
                            return false;
                        }

                        if ((int)req.resp.StatusCode == int.Parse(watcherItem.RAW))
                        {
                            req.resp?.Dispose();
                            return true;
                        }
                        else
                        {
                            req.resp?.Dispose();
                            return false;
                        }
                    }
                case "POST (BODY)":
                    {
                        StringContent sc = null;
                        if (watcherItem.Data != null) sc = new StringContent(watcherItem.Data);
                        var req = await getRawResponse(watcherItem.TypeCheck, watcherItem.Host, watcherItem.TimeCheck, watcherItem.Headers, sc);

                        if (req.err != null)
                        {
                            return false;
                        }

                        string resp = await req.resp.Content.ReadAsStringAsync();
                        if (resp.Equals(watcherItem.RAW))
                        {
                            req.resp?.Dispose();
                            return true;
                        }
                        else
                        {
                            req.resp?.Dispose();
                            return false;
                        }
                    }
                case "ICMP":
                    {
                        return await getICMPPing(watcherItem.Host, watcherItem.TimeCheck);
                    }
                case "TCP":
                    {
                        (string address, int port) data;
                        if (watcherItem.Host.Contains(':'))
                        {
                            try
                            {
                                string[] strings = watcherItem.Host.Split(':');
                                data.address = strings[0];
                                data.port = int.Parse(strings[1]);
                            }
                            catch
                            {
                                return false;
                            }
                        }
                        else
                        {
                            data.address = watcherItem.Host;
                            data.port = 80;
                        }

                        return await getTcpPing(data.address, watcherItem.TimeCheck, data.port);
                    }
                case "ARP":
                    return false;
                default: return false;
            }

        }

        async public static Task<bool> getTcpPing(string address, int timeout, int port = 80)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.ConnectAsync(new DnsEndPoint(address, port)).Wait(timeout);
                if (socket.Connected)
                {
                    socket.Close();
                    return true;
                }
                else
                {
                    socket.Close();
                    return false;
                }
            }
            catch
            {
                socket.Close();
                return false;
            }


        }

        async public static Task<bool> getICMPPing(string address, int timeout)
        {
            Ping pingSender = new Ping();

            byte[] data = Encoding.UTF8.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

            try
            {
                PingReply pingReply = await pingSender.SendPingAsync(address, timeout, data);
                if (pingReply.Status == IPStatus.Success)
                {
                    pingSender.Dispose();
                    return true;
                }
                else
                {
                    pingSender.Dispose();
                    return false;
                }
            }
            catch
            {
                pingSender.Dispose();
                return false;
            }


        }

        async public static Task<(HttpResponseMessage resp, string? err)> getRawResponse(string method, string url, int timeout, string[][] headers = null, StringContent stringContent = null)
        {
            string? err = null;
            HttpResponseMessage? response = null;

            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };

            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            HttpClient _client = new HttpClient(handler);
            _client.Timeout = new TimeSpan(0, 0, 0, 0, timeout);

            if (headers != null)
            {
                for (int i = 0; i < headers.Length; i++)
                {
                    _client.DefaultRequestHeaders.Add(headers[i][0], headers[i][1]);
                }
            }

            if (!url.Contains("http://") & !url.Contains("https://"))
            {
                url = "http://" + url;
            }

            if (method.ToUpper().Contains("GET"))
            {

                try
                {
                    response = await _client.GetAsync(url);
                }
                catch (Exception ex)
                {
                    err = "[ERROR]\n" + ex.Source + "\n" + ex.Message;
                }
                return (response, err);
            }
            else if (method.ToUpper().Contains("POST"))
            {
                try
                {
                    response = await _client.PostAsync(url, stringContent);
                }
                catch (Exception ex)
                {
                    err = "[ERROR]\n" + ex.Source + "\n" + ex.Message;
                }

                return (response, err);
            }
            else
            {
                return (null, null);
            }


        }
    }
}
