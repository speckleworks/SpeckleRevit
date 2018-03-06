using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpeckleRevitTest
{
    public partial class SpeckleRevitForm : Form
    {
        private UIApplication uiapp;
        public SpeckleRevitForm(ExternalCommandData commandData)
        {
            InitializeComponent();
            uiapp = commandData.Application;
        }
    }
}
