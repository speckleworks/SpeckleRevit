#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using CefSharp;
using System.IO;
using System.Reflection;
using CefSharp.WinForms;
using System.Windows.Forms;
#endregion

namespace SpeckleRevitPlugin
{
    [Transaction(TransactionMode.Manual)]
    public class ExtCmd : IExternalCommand
    {
        public static ChromiumWebBrowser Browser;

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            // Settings Helper Class
            // NOTE: You can use the AppMain.Settings throughout project to access active Revit Document, UIDocument, Application, etc.
            AppMain.Settings = new SettingsHelper(commandData);

            // SHOW DOCKABLE WINDOW
            //DockablePaneId m_dpID = GlobalHelper.MainDockablePaneId;
            //DockablePane m_dp = commandData.Application.GetDockablePane(m_dpID);
            //m_dp.Show();

            // initialise cef
            if (!Cef.IsInitialized)
                InitializeCef();

            // initialise one browser instance
            InitializeChromium();

            var form = new SpeckleRevitForm();

            form.Controls.Add(Browser);
            form.Show();

            return Result.Succeeded;
        }

        void InitializeCef()
        {

            Cef.EnableHighDPISupport();

            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var assemblyPath = Path.GetDirectoryName(assemblyLocation);
            var pathSubprocess = Path.Combine(assemblyPath, "CefSharp.BrowserSubprocess.exe");
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            var settings = new CefSettings
            {
                LogSeverity = LogSeverity.Verbose,
                LogFile = "ceflog.txt",
                BrowserSubprocessPath = pathSubprocess
            };

            // Initialize cef with the provided settings

            Cef.Initialize(settings);

        }

        public void InitializeChromium()
        {

#if DEBUG

            Browser = new ChromiumWebBrowser(@"http://localhost:9090/");

#else
        var path = Directory.GetParent(Assembly.GetExecutingAssembly().Location);
        Debug.WriteLine(path, "SPK");

        var indexPath = string.Format(@"{0}\app\index.html", path);

        if (!File.Exists(indexPath))
            Debug.WriteLine("Speckle for Revit: Error. The html file doesn't exists : {0}", "SPK");

        indexPath = indexPath.Replace("\\", "/");

        Browser = new ChromiumWebBrowser(indexPath);
#endif
            // Allow the use of local resources in the browser
            Browser.BrowserSettings = new BrowserSettings
            {
                FileAccessFromFileUrls = CefState.Enabled,
                UniversalAccessFromFileUrls = CefState.Enabled
            };


            Browser.Dock = DockStyle.Fill;
        }
    }
}