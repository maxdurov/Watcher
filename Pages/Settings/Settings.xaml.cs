using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace Watcher.Pages.Settings;

/// <summary>
/// Interaction logic for Settings.xaml
/// </summary>
public partial class Settings : Page
{
    private MainWindow _mainWindow;
    private bool IsCMOpen = false;
    public Settings(MainWindow context)
    {
        InitializeComponent();
        _mainWindow = context;
        Notifications.OnConfigUpdated += (() => { _InitCollectionOfWatchers(); });
        _InitCollectionOfWatchers();
        _InitStatus();
    }

    private void _InitCollectionOfWatchers()
    {
        var obj = DataManager.GetItems();
        Table.ItemsSource = obj;


    }

    private void _InitStatus()
    {
        string param = DataManager.GetStatusSetting();
        foreach (ComboBoxItem item in CB_STATUS.Items)
        {
            if (item.Tag.Equals(param))
            {
                CB_STATUS.Text = item.Content as string;
                break;
            }
        }
    }

    private void Table_add_Click(object sender, RoutedEventArgs e)
    {
        _mainWindow.MainFrame.Navigate(new SettingsEditor(_mainWindow));
    }

    private void ToBack_Click(object sender, RoutedEventArgs e)
    {
        _mainWindow.MainFrame.GoBack();

    }

    private void Table_change_Click(object sender, RoutedEventArgs e)
    {
        _mainWindow.MainFrame.Navigate(new SettingsEditor(_mainWindow, (WatcherItem)Table.SelectedItem));
    }

    private void Table_delete_Click(object sender, RoutedEventArgs e)
    {
        DataManager.DeleteItem((WatcherItem)Table.SelectedItem);
    }

    private void Table_SelectionChanged(object sender, RoutedEventArgs e)
    {
        var lb = sender as ListView;
        if (lb.SelectedIndex == -1)
        {
            Table_change.IsEnabled = false;
            Table_delete.IsEnabled = false;
        }
        else
        {
            Table_change.IsEnabled = true;
            Table_delete.IsEnabled = true;
        }
    }

    private void RebootSystemMonitoring_Click(object sender, RoutedEventArgs e)
    {
        _mainWindow.Engine.Restart();
        MessageBox.Show("Система перезагружена");
    }

    private void ResetConfig_Click(object sender, RoutedEventArgs e)
    {
        DataManager.ResetConfError();
    }

    private void CB_STATUS_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsCMOpen)
        {
            ComboBox cb = sender as ComboBox;
            DataManager.SetStatusSetting((string)((ComboBoxItem)cb.SelectedItem).Tag);
            IsCMOpen = false;
        }

    }

    private void CB_STATUS_DropDownOpened(object sender, EventArgs e)
    {
        IsCMOpen = true;
    }

    private void Faq_Click(object sender, RoutedEventArgs e)
    {
        new FAQ().Show();
    }
}
