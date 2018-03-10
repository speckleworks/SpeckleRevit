#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion

namespace SpeckleRevitPlugin
{
    [Transaction(TransactionMode.Manual)]
    public class ExtCmd : IExternalCommand
    {
        //public static ChromiumWebBrowser Browser;

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            // Settings Helper Class
            // NOTE: You can use the AppMain.Settings throughout project to access active Revit Document, UIDocument, Application, etc.
            AppMain.Settings = new SettingsHelper(commandData);

            // SHOW DOCKABLE WINDOW
            DockablePaneId m_dpID = GlobalHelper.MainDockablePaneId;
            DockablePane m_dp = commandData.Application.GetDockablePane(m_dpID);

            m_dp.Show();

            return Result.Succeeded;
        }
    }
}
