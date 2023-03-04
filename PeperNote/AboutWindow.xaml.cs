using System;
using System.Reflection;
using System.Windows;

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

            Title=$"About PeperNote v{Assembly.GetEntryAssembly().GetName().Version}";

            // https://learn.microsoft.com/en-us/dotnet/desktop/wpf/app-development/wpf-application-resource-content-and-data-files
            var info = Application.GetResourceStream(new Uri("/Resources/About.rtf", UriKind.Relative));
            RichTextBoxAbout.Selection.Load(info.Stream, DataFormats.Rtf);
        }
    }
}
