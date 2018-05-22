#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SpeckleRevitPlugin.Classes;

#endregion

namespace SpeckleRevitPlugin.Entry
{
    [Transaction(TransactionMode.Manual)]
    public class ExtCmd : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            // Settings Helper Class
            // NOTE: You can use the AppMain.Settings throughout project to access active Revit Document, UIDocument, Application, etc.
            //AppMain.Settings = new SettingsHelper(commandData);

            // SHOW DOCKABLE WINDOW
            var m_dpID = GlobalHelper.MainDockablePaneId;
            var m_dp = commandData.Application.GetDockablePane(m_dpID);

            m_dp.Show();

            return Result.Succeeded;
        }
    }
}
