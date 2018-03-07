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

namespace SpeckleRevitPlugin
{
    public partial class SpeckleRevitForm : Form
    {
        private clsSettings _settings;

        /// <summary>
        /// Speckle Revit Form
        /// </summary>
        /// <param name="settings"></param>
        public SpeckleRevitForm(clsSettings settings)
        {
            InitializeComponent();
            _settings = settings;
        }
    }
}
