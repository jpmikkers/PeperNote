using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Resources;
using System.Windows.Shapes;

namespace PeperNote
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            // https://learn.microsoft.com/en-us/dotnet/desktop/wpf/app-development/wpf-application-resource-content-and-data-files
            var info = Application.GetResourceStream(new Uri("/Resources/About.rtf", UriKind.Relative));
            RichTextBoxAbout.Selection.Load(info.Stream, DataFormats.Rtf);
        }
    }
}
