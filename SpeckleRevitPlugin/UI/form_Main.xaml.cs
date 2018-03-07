#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
#endregion

namespace SpeckleRevitPlugin
{
    /// <summary>
    /// Interaction logic for form_Main.xaml
    /// </summary>
    public partial class form_Main : Window
    {
        private SettingsHelper _s;

        public form_Main(SettingsHelper s)
        {
            InitializeComponent();
            Label_Version.Content = "v." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            //widen scope
            _s = s;
        }

        #region Form Events
        /// <summary>
        /// Close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Run code here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Run_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Drag Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        #endregion
    }
}
