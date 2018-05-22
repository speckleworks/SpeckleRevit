#region Namespaces
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using SpeckleRevitPlugin.Classes;
using SpeckleRevitPlugin.UI;
using SpeckleRevitPlugin.Utilities;
#endregion

namespace SpeckleRevitPlugin.Entry
{
    [Transaction(TransactionMode.Manual)]
    public class AppMain : IExternalApplication
    {
        private static readonly string m_Path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static UIControlledApplication uiApp;
        private static AppMain _thisApp;
        public static ExternalEvent SpeckleEvent;
        public static SpeckleRequestHandler SpeckleHandler = new SpeckleRequestHandler();

        internal static FormMainDock MainDock;
        internal DockablePaneProviderData DockData;
        //internal static SettingsHelper Settings { get; set; }

        public delegate void ModelSynched();
        public static event ModelSynched OnModelSynched;

        public Result OnStartup(UIControlledApplication a)
        {
            try
            {
                uiApp = a;
                _thisApp = this;

                if (MainDock == null)
                {
                    MainDock = new FormMainDock();
                    DockData = new DockablePaneProviderData
                    {
                        FrameworkElement = MainDock,
                        InitialState = new DockablePaneState
                        {
                            DockPosition = DockPosition.Right
                        }
                    };
                }

                uiApp.RegisterDockablePane(GlobalHelper.MainDockablePaneId, GlobalHelper.MainPanelName(), MainDock);

                // (Konrad) We are going to use this External Event Handler all across Speckle
                // It's best to keep it on the main app, and keep it public.
                SpeckleHandler = new SpeckleRequestHandler();
                SpeckleEvent = ExternalEvent.Create(SpeckleHandler);

                a.ControlledApplication.DocumentCreated += OnDocumentCreated;
                a.ControlledApplication.DocumentOpened += OnDocumentOpened;
                a.ControlledApplication.DocumentSynchronizedWithCentral += OnDocumentSynchronized;
                a.ControlledApplication.DocumentSaving += OnDocumentSaving;

                AddRibbonPanel(a);

                return Result.Succeeded;
            }
            catch
            {
                return Result.Failed;
            }
        }

        private static void OnDocumentSaving(object sender, DocumentSavingEventArgs e)
        {
            var doc = e.Document;
            if (doc == null || doc.IsFamilyDocument) return;

            OnModelSynched?.Invoke();
        }

        private static void OnDocumentSynchronized(object sender, DocumentSynchronizedWithCentralEventArgs e)
        {
            var doc = e.Document;
            if (doc == null || doc.IsFamilyDocument) return;

            OnModelSynched?.Invoke();
        }

        private void OnDocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            var doc = e.Document;
            if (doc == null || doc.IsFamilyDocument)
            {
                HideDockablePane();
            }

            // TODO: In theory this means that we either opened a new doc or another doc. We need to re-instantiate the speckle panel/clients for a new doc
        }

        private void OnDocumentCreated(object sender, DocumentCreatedEventArgs e)
        {
            var doc = e.Document;
            if (doc == null || doc.IsFamilyDocument)
            {
                HideDockablePane();
            }

            // TODO: In theory this means that we either opened a new doc or another doc. We need to re-instantiate the speckle panel/clients for a new doc
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        #region Utilities

        /// <summary>
        /// Load an Image Source from File
        /// </summary>
        /// <param name="SourceName"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private ImageSource LoadPngImgSource(string SourceName)
        {
            try
            {
                // Assembly
                var assembly = Assembly.GetExecutingAssembly();

                // Stream
                var icon = assembly.GetManifestResourceStream(SourceName);

                // Decoder
                var decoder = new PngBitmapDecoder(icon, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

                // Source
                ImageSource m_source = decoder.Frames[0];
                return (m_source);
            }
            catch
            {
                // ignored
            }

            return null;
        }

        /// <summary>
        /// Add the Ribbon Item and Panel
        /// </summary>
        /// <param name="a"></param>
        /// <remarks></remarks>
        public void AddRibbonPanel(UIControlledApplication a)
        {
            try
            {
                // First Create the Tab
                a.CreateRibbonTab("Speckle");
            }
            catch
            {
                // Might already exist...
            }

            // Tools
            AddButton("Speckle",
                    "Plugin\r\nTest",
                    "Plugin\r\nTest",
                    "SpeckleRevitPlugin.Resources.Template_16.png",
                    "SpeckleRevitPlugin.Resources.Template_32.png",
                    (m_Path + "\\SpeckleRevitPlugin.dll"),
                    "SpeckleRevitPlugin.Entry.ExtCmd",
                    "Speckle connection test for Revit.");
        }

        /// <summary>
        /// Add a button to a Ribbon Tab
        /// </summary>
        /// <param name="Rpanel">The name of the ribbon panel</param>
        /// <param name="ButtonName">The Name of the Button</param>
        /// <param name="ButtonText">Command Text</param>
        /// <param name="ImagePath16">Small Image</param>
        /// <param name="ImagePath32">Large Image</param>
        /// <param name="dllPath">Path to the DLL file</param>
        /// <param name="dllClass">Full qualified class descriptor</param>
        /// <param name="Tooltip">Tooltip to add to the button</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool AddButton(string Rpanel, string ButtonName, string ButtonText, string ImagePath16, string ImagePath32, string dllPath, string dllClass, string Tooltip)
        {
            try
            {
                // The Ribbon Panel
                RibbonPanel ribbonPanel = null;

                // Find the Panel within the Case Tab
                var rp = new List<RibbonPanel>();
                rp = uiApp.GetRibbonPanels("Speckle");
                foreach (RibbonPanel x in rp)
                {
                    if (x.Name.ToUpper() == Rpanel.ToUpper())
                    {
                        ribbonPanel = x;
                    }
                }

                // Create the Panel if it doesn't Exist
                if (ribbonPanel == null)
                {
                    ribbonPanel = uiApp.CreateRibbonPanel("Speckle", Rpanel);
                }

                // Create the Pushbutton Data
                var pushButtonData = new PushButtonData(ButtonName, ButtonText, dllPath, dllClass);
                if (!string.IsNullOrEmpty(ImagePath16))
                {
                    try
                    {
                        pushButtonData.Image = LoadPngImgSource(ImagePath16);
                    }
                    catch
                    {
                        Debug.WriteLine("Image not found", "SPK");
                    }
                }
                if (!string.IsNullOrEmpty(ImagePath32))
                {
                    try
                    {
                        pushButtonData.LargeImage = LoadPngImgSource(ImagePath32);
                    }
                    catch
                    {
                        Debug.WriteLine("Image not found", "SPK");
                    }
                }
                pushButtonData.ToolTip = Tooltip;

                // Add the button to the tab
                var unused = (PushButton)ribbonPanel.AddItem(pushButtonData);
            }
            catch
            {
                // ignored
            }
            return true;
        }

        /// <summary>
        /// Close the Dockable Pane
        /// </summary>
        internal void HideDockablePane()
        {
            try
            {
                var m_dp = uiApp.GetDockablePane(GlobalHelper.MainDockablePaneId);
                m_dp.Hide();
            }
            catch
            {
                // ignored
            }
        }

        #endregion
    }
}
