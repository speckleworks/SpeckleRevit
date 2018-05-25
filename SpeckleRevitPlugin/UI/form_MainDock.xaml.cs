#region Namespaces
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using Autodesk.Revit.UI;
using CefSharp;
using SpeckleRevitPlugin.Classes;
using SpeckleRevitPlugin.Entry;

#endregion

namespace SpeckleRevitPlugin.UI
{
    /// <summary>
    /// Interaction logic for form_MainDock.xaml
    /// </summary>
    public partial class FormMainDock : IDockablePaneProvider
    {
        public static Interop Store;

        /// <summary>
        /// Main Dockable window
        /// </summary>
        public FormMainDock()
        {
            InitializeCef();
            InitializeComponent();
            InitializeChromium();

            // initialise one store
            Store = new Interop(Browser);

            // make them talk together
            Browser.RegisterAsyncJsObject("Interop", Store);

            //_extEvent = ExternalEvent.Create(_handler);
        }

        private static void InitializeCef()
        {
            Cef.EnableHighDPISupport();

            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var assemblyPath = Path.GetDirectoryName(assemblyLocation);
            if (assemblyPath == null) return;

            var pathSubprocess = Path.Combine(assemblyPath, "CefSharp.BrowserSubprocess.exe");
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            var settings = new CefSettings
            {
                LogSeverity = LogSeverity.Verbose,
                LogFile = "ceflog.txt",
                BrowserSubprocessPath = pathSubprocess,
                RemoteDebuggingPort = 8088
            };

            // Initialize cef with the provided settings
            settings.CefCommandLineArgs.Add("allow-file-access-from-files", "1");
            settings.CefCommandLineArgs.Add("disable-web-security", "1");
            Cef.Initialize(settings);
        }
        public void InitializeChromium()
        {

#if DEBUG
            Browser.Address = @"http://localhost:2020/";

#else
            var path = Directory.GetParent(Assembly.GetExecutingAssembly().Location);
            Debug.WriteLine(path, "SPK");

            var indexPath = string.Format(@"{0}\app\index.html", path);

            if (!File.Exists(indexPath))
                Debug.WriteLine("Speckle for Revit: Error. The html file doesn't exists : {0}", "SPK");

            indexPath = indexPath.Replace("\\", "/");

            Browser.Address = indexPath;
#endif
            //Allow the use of local resources in the browser
            Browser.BrowserSettings = new BrowserSettings
            {
                FileAccessFromFileUrls = CefState.Enabled,
                UniversalAccessFromFileUrls = CefState.Enabled
            };
        }


        #region Form Events

        /// <summary>
        /// Can't show the Dev Tools panel until window finishes initialization.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_MainDock_OnLoaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            //Browser.ShowDevTools();
#endif
        }

        /// <summary>
        /// Safely raise an external command transaction event (for modeless)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Button_ExampleTransaction_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                //textBox_Comments.Text;
                //AppMain.Settings.CommandType = EnumCommandType.Command1;

                // Safe Update
                //_extEvent.Raise();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        #endregion

        #region Dockable Pane

        /// <summary>
        /// IDockablePaneProvider Implementation
        /// </summary>
        /// <param name="data"></param>
        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this;
            var unused = new DockablePaneProviderData();
            data.InitialState = new DockablePaneState
            {
                DockPosition = DockPosition.Right
            };
        }

        #endregion
    }
}
