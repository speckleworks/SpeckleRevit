#region Namespaces
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace SpeckleRevitPlugin
{
    class GlobalHelper
    {
        public static DockablePaneId MainDockablePaneId = new DockablePaneId(new Guid("{0cf3e223-608e-4f68-903d-f319168378f5}"));
        public static string MainPanelName()
        {
            return "Speckle for Revit v." + Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
