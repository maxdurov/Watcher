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

namespace Watcher.Pages.Settings
{
    /// <summary>
    /// Interaction logic for PopUp.xaml
    /// </summary>
    public partial class PopUp : Window
    {
        public string HeaderName { get; set; }
        public string Instruction {  get; set; }

        public string? Result = null;
        public PopUp(string headname, string instruction)
        {
            InitializeComponent();
            this.DataContext = this;
            HeaderName = headname;
            Instruction = instruction;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Result = FieldResult.Text;
            Close();
        }
    }
}
