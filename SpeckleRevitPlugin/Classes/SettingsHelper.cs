#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SpeckleRevitPlugin.Entry;
#endregion

namespace SpeckleRevitPlugin.Classes
{
    //public class SettingsHelper
    //{
    //    //private readonly ExternalCommandData _cmd;
    //    private Application _app;
    //    private Document _doc;

    //    ///// <summary>
    //    ///// Constructor
    //    ///// </summary>
    //    ///// <param name="cmd"></param>
    //    //public SettingsHelper(ExternalCommandData cmd)
    //    //{
    //    //    _doc = null;
    //    //    _cmd = cmd;
    //    //}

    //    /// <summary>
    //    /// Constructor - from application (events)
    //    /// </summary>
    //    /// <param name="d"></param>
    //    /// <param name="a"></param>
    //    public SettingsHelper(Document d, Application a)
    //    {
    //        _app = a;
    //        _doc = d;
    //    }

    //    #region Command Data

    //    /// <summary>
    //    /// Revit UI application
    //    /// </summary>
    //    public UIApplication UiApp
    //    {
    //        get
    //        {
    //            try
    //            {
    //                return _doc.Application.;
    //            }
    //            catch
    //            {
    //                // ignored
    //            }

    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// Revit Application
    //    /// </summary>
    //    public Application App
    //    {
    //        get
    //        {
    //            try
    //            {
    //                return _cmd.Application.Application;
    //            }
    //            catch
    //            {
    //                // ignored
    //            }

    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// Revit UI Document
    //    /// </summary>
    //    public UIDocument UiDoc
    //    {
    //        get
    //        {
    //            try
    //            {
    //                return _cmd.Application.ActiveUIDocument;
    //            }
    //            catch
    //            {
    //                // ignored
    //            }

    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// Revit Document
    //    /// </summary>
    //    public Document ActiveDoc
    //    {
    //        get
    //        {
    //            try
    //            {
    //                return UiDoc.Document;
    //            }
    //            catch
    //            {
    //                // ignored
    //            }

    //            return null;
    //        }
    //    }

    //    #endregion

    //    #region Public Properties - Modeless

    //    internal EnumCommandType CommandType { get; set; }

    //    #endregion
    //}
}
