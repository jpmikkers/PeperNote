using System;
using WixSharp;
using WixSharp.UI.Forms;
using WixSharp.UI;
using WixSharp.Forms;
using WixSharp.CommonTasks;
using Microsoft.Deployment.WindowsInstaller;
using System.Windows.Forms;

namespace PeperNoteWixSharp
{
    class Program
    {
        static void Main()
        {
            var project = new Project("PeperNote",

                new Property("INSTALLSCOPE", "PERMACHINE"),
                new Property("ALLUSERS", "1"),
                new Property("MSIINSTALLPERUSER", "0"),

                new Dir(@"%ProgramFiles%\JPMikkers\PeperNote",
                    new File(@"..\PeperNote\bin\release\PeperNote.exe"),
                    new File(@"..\PeperNote\bin\release\PeperNote.exe.config")),

                new Dir(@"%ProgramMenu%\JPMikkers\PeperNote",
                    new ExeFileShortcut("PeperNote", "[INSTALLDIR]PeperNote.exe", arguments: ""),
                    new ExeFileShortcut("Uninstall PeperNote", "[System64Folder]msiexec.exe", "/x [ProductCode]")),

                new Dir(@"%StartupFolder%",
                    new ExeFileShortcut("PeperNote", "[INSTALLDIR]PeperNote.exe", arguments: ""))

                // ,new ManagedAction("MyAction")
                // ,new Property("InstallScope", "PerMachine")
            );

            project.UI = WUI.WixUI_ProgressOnly;
            project.GUID = new Guid("7A24B736-7905-434C-948F-3A6E7CE65A81");
            project.LicenceFile = @"..\PeperNote\License.rtf";
            project.ControlPanelInfo.ProductIcon = @"..\PeperNote\MainIcon.ico";
            project.ControlPanelInfo.Manufacturer = "JPMikkers";
            project.ControlPanelInfo.UrlInfoAbout = @"https://github.com/jpmikkers/pepernote";
            project.ControlPanelInfo.UrlUpdateInfo = @"https://github.com/jpmikkers/pepernote";
            project.ControlPanelInfo.HelpLink = @"https://github.com/jpmikkers/pepernote";
            project.Description = @"PeperNote is an open source sticky note desktop application";
            project.Version = new Version(1, 0);

            //project.Properties
            //project.ManagedUI = new ManagedUI();
            //project.ManagedUI.InstallDialogs.Add(Dialogs.Licence);
            //                                .Add(Dialogs.InstallScope)
            //                                .Add(Dialogs.Progress)
            //                                .Add(Dialogs.Exit);
            //project.ManagedUI.ModifyDialogs.Add(Dialogs.Progress)
            //                                .Add(Dialogs.Exit);
            //project.SourceBaseDir = "<input dir path>";
            //project.OutDir = "<output dir path>";

            project.BuildMsi();
        }
    }


    //public class CustomActions
    //{
    //    [CustomAction]
    //    public static ActionResult MyAction(Session session)
    //    {
    //        //MessageBox.Show(System.IO.Path.Combine(session["INSTALLDIR"], "PeperNote.exe"), "Embedded Managed CA");
    //        return ActionResult.Success;
    //    }
    //}
}