#region Namespaces
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

using Visibility = System.Windows.Visibility;
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
            InitializeComponent();
            _extEvent = ExternalEvent.Create(_handler);
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
