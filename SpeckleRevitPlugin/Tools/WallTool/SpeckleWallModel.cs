using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using SpeckleRevitPlugin.UI;

namespace SpeckleRevitPlugin.Tools.WallTool
{
    public class SpeckleWallModel
    {
        private Document Doc { get; set; }

        public SpeckleWallModel(Document doc)
        {
            Doc = doc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<InputWrapper> GetAllAvailableInputs()
        {
            var result = new HashSet<InputWrapper>();

            foreach (var wt in new FilteredElementCollector(Doc).OfClass(typeof(WallType)))
            {
                foreach (Parameter p in wt.Parameters)
                {
                    if(p.IsReadOnly) continue;

                    var iw = new InputWrapper(p, false);
                    if(iw.DataType != LocalDataType.Boolean) continue; //TODO: just for now

                    result.Add(iw);
                }
            }

            return new ObservableCollection<InputWrapper>(result);
        }
    }
}
