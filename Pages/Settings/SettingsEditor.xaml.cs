using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Watcher.Classes;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Watcher.Pages.Settings
{
    /// <summary>
    /// Interaction logic for SettingsEditor.xaml
    /// </summary>
    public partial class SettingsEditor : Page
    {
        private MainWindow _mainWindows;
        private string[][] _headers;
        private string _body_post;
        private WatcherItem editing;
        public SettingsEditor(MainWindow context, WatcherItem item = null)
        {
            InitializeComponent();
            _mainWindows = context;
            if (item != null)
            {
                editing = item;
                FillForm(item);
            }
            Method_SelectionChanged(Method, null);
        }

        private void FillForm(WatcherItem item)
        {
            Title.Text = item.Title;
            Address.Text = item.Host;
            Method.Text = item.TypeCheck;
            Response.Text = item.RAW;
            _body_post = item.Data;
            _headers = item.Headers;
            isCritical.IsChecked = item.IsCritical;
            TimeCheck.Text = (item.TimeCheck / 1000).ToString();

        }

        private void ToBack_Click(object sender, RoutedEventArgs e)
        {
            _mainWindows.MainFrame.GoBack();
        }

        private void Method_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Response == null) return;

            ComboBox comboBox = sender as ComboBox;

            Response.IsEnabled = false;
            ToGetRespRaw.IsEnabled = false;
            ToAddHeader.IsEnabled = false;
            ToAddBody.IsEnabled = false;

            _headers = null;
            _body_post = null;



            switch (((ComboBoxItem)comboBox.SelectedItem).Content)
            {
                case "GET (CODE)":
                    Response.IsEnabled = true;
                    ToAddHeader.IsEnabled = true;
                    ToGetRespRaw.IsEnabled = true;

                    break;
                case "GET (BODY)":
                    Response.IsEnabled = true;
                    ToAddHeader.IsEnabled = true;
                    ToGetRespRaw.IsEnabled = true;

                    break;
                case "POST (CODE)":
                    Response.IsEnabled = true;
                    ToAddHeader.IsEnabled = true;
                    ToAddBody.IsEnabled = true;
                    ToGetRespRaw.IsEnabled = true;

                    break;
                case "POST (BODY)":
                    Response.IsEnabled = true;
                    ToAddHeader.IsEnabled = true;
                    ToAddBody.IsEnabled = true;
                    ToGetRespRaw.IsEnabled = true;

                    break;
                default:
                    Response.Clear();
                    break;

            }
        }

        private void Address_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text.Length != 0)
            {
                Address_tip.Visibility = Visibility.Hidden;
            }
            else
            {
                Address_tip.Visibility = Visibility.Visible;
            }
        }

        async private void ToGetRespRaw_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            ToSave.IsEnabled = false;
            StringContent sc = null;
            if (_body_post != null) sc = new StringContent(_body_post);
            var req = await WatcherEngine.getRawResponse(Method.Text, Address.Text, 5000, _headers, sc);

            if (req.resp == null)
            {

                Response.Text = req.err;
                req.resp?.Dispose();
                (sender as Button).IsEnabled = true;
                ToSave.IsEnabled = true;
                return;

            }

            switch (Method.Text)
            {
                case "GET (CODE)":
                    Response.Text = ((int)req.resp.StatusCode).ToString();
                    break;
                case "GET (BODY)":
                    Response.Text = await req.resp.Content.ReadAsStringAsync();
                    break;
                case "POST (CODE)":
                    Response.Text = ((int)req.resp.StatusCode).ToString();
                    break;
                case "POST (BODY)":
                    Response.Text = await req.resp.Content.ReadAsStringAsync();
                    break;
            }

            req.resp.Dispose();
            (sender as Button).IsEnabled = true;
            ToSave.IsEnabled = true;
            return;


        }

        private void ToAddHeader_Click(object sender, RoutedEventArgs e)
        {
            PopUp popUp = new PopUp("Настройка Headers", "Введите необходимые Headers для запроса. Разделение происходит по символу \";\". \nПримеры заголовков: HOST: www.google.com, Authorization: Bearer mytoken123; Token: 123;");
            if (_headers != null)
            {
                string headers = "";

                for (int i = 0; i < _headers.Length; i++)
                {
                    headers += _headers[i][0] + ": " + _headers[i][1] + "; ";
                }

                popUp.FieldResult.Text = headers;
            }
            popUp.ShowDialog();

            if (popUp.Result == null) return;

            string[] res = Regex.Replace(popUp.Result, @"\s", "").Split(';');

            List<string[]> handled = new List<string[]>();

            for (int i = 0; i < res.Length; i++)
            {
                if (!res[i].Equals(""))
                {
                    string source = Regex.Replace(res[i], @"\s", "");
                    string[] arr = source.Split(':');

                    for (int ia = 0; ia < arr.Length; ia++)
                    {
                        arr[ia] = arr[ia].Replace(" ", "");
                    }
                    handled.Add(arr);

                }

            }

            _headers = handled.ToArray();
        }

        private void ToAddBody_Click(object sender, RoutedEventArgs e)
        {
            PopUp popUp = new PopUp("Настройка Body", "Введите данные для POST запроса");
            if (_body_post != null)
            {
                popUp.FieldResult.Text = _body_post;
            }
            popUp.ShowDialog();
            if (popUp.Result == null) return;
            string result = popUp.Result;
            _body_post = result;
        }

        private void ToSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Title.Text) || string.IsNullOrEmpty(Address.Text) || string.IsNullOrEmpty(TimeCheck.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            WatcherItem wi = new WatcherItem(Title.Text, Address.Text, Method.Text, Response.Text, _body_post, _headers, (bool)isCritical.IsChecked, int.Parse(TimeCheck.Text) * 1000);

            if (editing != null)
            {
                if (DataManager.EditItem(wi, editing.Title))
                {
                    _mainWindows.MainFrame.GoBack();
                    return;
                }
                else
                {
                    MessageBox.Show("Не удалось изменить объект.");
                    return;
                }
            }

            var resp = DataManager.AddItem(wi);
            if (resp.result)
            {

                _mainWindows.MainFrame.GoBack();
            }
            else
            {
                MessageBox.Show(resp.reason);
            }
        }

        private void TimeCheck_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Debug.WriteLine(e);

            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Left || e.Key == Key.Right)
            {
                e.Handled = false;
            }
            else if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
