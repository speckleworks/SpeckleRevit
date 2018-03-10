#region Namespaces
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using CefSharp;
using Visibility = System.Windows.Visibility;
using Path = System.IO.Path;
using System.IO;
using System.Diagnostics;
#endregion

namespace SpeckleRevitPlugin
{
    /// <summary>
    /// Interaction logic for form_MainDock.xaml
    /// </summary>
    public partial class form_MainDock : Page, IDockablePaneProvider
    {
        private ExternalEvent _extEvent;
        private ExtCmdModeless _handler = new ExtCmdModeless();

        /// <summary>
        /// Main Dockable window
        /// </summary>
        public form_MainDock()
        {
            InitializeCef();
            InitializeComponent();
            InitializeChromium();
            _extEvent = ExternalEvent.Create(_handler);
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

            Browser.Address = @"http://localhost:9090/";

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


            //Browser.Dock = DockStyle.Fill;
        }


        #region Form Events
        /// <summary>
        /// Safely raise an external command transaction event (for modeless)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ExampleTransaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //textBox_Comments.Text;
                AppMain.Settings.CommandType = EnumCommandType.Command1;

                // Safe Update
                _extEvent.Raise();
            }
            catch { }
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
            DockablePaneProviderData d = new DockablePaneProviderData();
            data.InitialState = new DockablePaneState();
            data.InitialState.DockPosition = DockPosition.Right;
        }
        #endregion
    }
}
