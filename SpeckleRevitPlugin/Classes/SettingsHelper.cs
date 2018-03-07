#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace SpeckleRevitPlugin
{
    public class SettingsHelper
    {
        private ExternalCommandData _cmd;
        private Application _app;
        private Document _doc;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cmd"></param>
        public SettingsHelper(ExternalCommandData cmd)
        {
            _doc = null;
            _cmd = cmd;
        }

        /// <summary>
        /// Constructor - from application (events)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="a"></param>
        public SettingsHelper(Document d, Application a)
        {
            _app = a;
            _doc = d;
        }

        #region Command Data
        /// <summary>
        /// Revit UI application
        /// </summary>
        public UIApplication UiApp
        {
            get
            {
                try
                {
                    return _cmd.Application;
                }
                catch { }
                return null;
            }
        }

        /// <summary>
        /// Revit Application
        /// </summary>
        public Application App
        {
            get
            {
                try
                {
                    return _cmd.Application.Application;
                }
                catch { }
                return null;
            }
        }

        /// <summary>
        /// Revit UI Document
        /// </summary>
        public UIDocument UiDoc
        {
            get
            {
                try
                {
                    return _cmd.Application.ActiveUIDocument;
                }
                catch { }
                return null;
            }
        }

        /// <summary>
        /// Revit Document
        /// </summary>
        public Document ActiveDoc
        {
            get
            {
                try
                {
                    return UiDoc.Document;
                }
                catch { }
                return null;
            }
        }
        #endregion

        #region Public Properties - Modeless
        internal EnumCommandType CommandType { get; set; }
        #endregion
    }
}