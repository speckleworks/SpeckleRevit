#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace SpeckleRevitPlugin
{
    public enum EnumCommandType : int
    {
        Command1,
        Command2,
        Command3
    }

    /// <summary>
    /// Model Updates for Modeless Dialog
    /// </summary>
    /// <param name="app"></param>
    class ExtCmdModeless : IExternalEventHandler
    {
        /// <summary>
        /// Model Updates for Modeless Dialog
        /// </summary>
        /// <param name="app"></param>
        public void Execute(UIApplication uiapp)
        {
            switch (AppMain.Settings.CommandType)
            {
                case EnumCommandType.Command1:
                    SampleCommand1(uiapp);
                    break;
                case EnumCommandType.Command2:
                    SampleCommand2(uiapp);
                    break;
                case EnumCommandType.Command3:
                    SampleCommand3(uiapp);
                    break;
            }
        }

        /// <summary>
        /// Command 1
        /// </summary>
        /// <param name="app"></param>
        private void SampleCommand1(UIApplication uiapp)
        {
            try
            {
                using (Transaction t = new Transaction(AppMain.Settings.ActiveDoc, "Do something"))
                {
                    t.Start();

                    t.Commit();
                }
            }
            catch { }
        }

        /// <summary>
        /// Command 2
        /// </summary>
        /// <param name="app"></param>
        private void SampleCommand2(UIApplication uiapp)
        {
            try
            {
                using (Transaction t = new Transaction(AppMain.Settings.ActiveDoc, "Do something"))
                {
                    t.Start();

                    t.Commit();
                }
            }
            catch { }
        }

        /// <summary>
        /// Command 3
        /// </summary>
        /// <param name="app"></param>
        private void SampleCommand3(UIApplication uiapp)
        {
            try
            {
                using (Transaction t = new Transaction(AppMain.Settings.ActiveDoc, "Do something"))
                {
                    t.Start();

                    t.Commit();
                }
            }
            catch  { }
        }

        /// <summary>
        /// External event anme
        /// </summary>
        /// <returns></returns>
        public string GetName() { return "ExtCmdModeless"; }
    }
}
