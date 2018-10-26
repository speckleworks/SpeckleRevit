using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace SpeckleRevitPlugin.UI.BooleanSelector
{
    public class BooleanSelectorViewModel : ViewModelBase
    {
        public BooleanSelectorModel Model { get; set; }

        public BooleanSelectorViewModel(BooleanSelectorModel model)
        {
            Model = model;
        }
    }
}
