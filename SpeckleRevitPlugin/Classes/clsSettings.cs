using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace SpeckleRevitPlugin
{
    public class clsSettings
    {
        private ExternalCommandData _cmd;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cmd"></param>
        public clsSettings(ExternalCommandData cmd)
        {

            // Widen Scope
            _cmd = cmd;

        }

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
        public Document Doc
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
    }
}