using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace PeperNote
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private const string AutoRunRegKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public SettingsWindow()
        {
            InitializeComponent();

            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(AutoRunRegKey, false))
                {
                    Assembly curAssembly = Assembly.GetExecutingAssembly();
                    checkBoxAutoRun.IsChecked = ((string)key.GetValue(curAssembly.GetName().Name)) == curAssembly.Location;
                }
            }
            catch
            {
                checkBoxAutoRun.IsChecked = false;
            }
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            var curAssembly = Assembly.GetExecutingAssembly();

            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(AutoRunRegKey, true))
                {
                    if (checkBoxAutoRun.IsChecked ?? false)
                    {
                        key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
                    }
                    else 
                    {
                        key.DeleteValue(curAssembly.GetName().Name);
                    }
                }
            }
            catch
            {
            }
            Close();
        }
    }
}
