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
using System.IO;
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;

namespace PeperNote
{
    public partial class MainWindow : Window
    {
        private static string applicationName = "PeperNote";
        private string applicationFolder;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private readonly Mutex applicationMutex;
        private readonly bool firstInstance;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                applicationMutex = new Mutex(true, $"{applicationName}{System.Security.Principal.WindowsIdentity.GetCurrent().User.AccountDomainSid}", out bool createdNew);
                firstInstance = createdNew;
            }
            catch
            {
                firstInstance = true;
            }

            var lad = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            applicationFolder = System.IO.Path.Combine(lad, applicationName);

            if(!Directory.Exists(applicationFolder))
            {
                Directory.CreateDirectory(applicationFolder);
            }

            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            //Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/YourReferencedAssembly;component/YourPossibleSubFolder/YourResourceFile.ico")).Stream;
            notifyIcon.Icon = Properties.Resources.TrayIcon;
            notifyIcon.Visible = true;
            notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                new System.Windows.Forms.ToolStripMenuItem("New note",null,new EventHandler((o,a) => CreateNewNote())),
                new System.Windows.Forms.ToolStripMenuItem("Bring notes to front",null,new EventHandler((o,a) => BringNotesToFront())),
             // new System.Windows.Forms.MenuItem("Settings",new EventHandler((o,a) => ShowSettings())),
                new System.Windows.Forms.ToolStripMenuItem("About PeperNote",null,new EventHandler((o,a) => ShowAbout())),
                new System.Windows.Forms.ToolStripSeparator(),
                new System.Windows.Forms.ToolStripMenuItem("Exit",null,new EventHandler((o,a) => CloseAction())),
            });
            this.Dispatcher.BeginInvoke(new Action(LoadNotes), DispatcherPriority.Background);
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            CreateNewNote();
        }

        private void MyNotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            CreateNewNote();
        }

        private void CreateNewNote()
        {
            var nw = new NoteWindow(System.IO.Path.Combine(applicationFolder, Guid.NewGuid().ToString()));
            nw.Owner = this;
            nw.ShowActivated = true;
            nw.Show();
        }

        private void CloseAction()
        {
            if (MessageBox.Show(this,"Are you sure you want to exit PeperNote ?","Exit PeperNote",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private static void BringNotesToFront()
        {
            foreach (Window w in App.Current.Windows.OfType<NoteWindow>())
            {
                HideAndShowWindowHelper.ShiftWindowIntoForeground(w);
            }
        }

        private void ShowSettings()
        {
            var sw = new SettingsWindow();
            sw.Owner = this;
            sw.ShowDialog();
        }

        private void ShowAbout()
        {
            var sw = new AboutWindow();
            sw.Owner = this;
            sw.ShowDialog();
        }

        private void LoadNotes()
        {
            if (!firstInstance)
            {
                Close();
                return;
            }

            foreach (var noteSettingsFile in Directory.EnumerateFiles(applicationFolder, "*.xml", SearchOption.TopDirectoryOnly))
            {
                var note = new NoteWindow(noteSettingsFile);
                note.Owner = this;
                note.ShowActivated = false;
                note.Show();
            }

            // Just hiding the window is not sufficient, as it still temporarily pops up the first time. Therefore, make it transparent.
            // AllowsTransparency = true;
            // Background = Brushes.Transparent;
            // WindowStyle = WindowStyle.None;
            Visibility = Visibility.Hidden;
            ShowInTaskbar = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if(notifyIcon!=null)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
                notifyIcon = null;
            }
        }
    }
}
