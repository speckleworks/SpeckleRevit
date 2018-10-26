#region Namespaces
using System.Windows;
using System.Windows.Input;
#endregion

namespace SpeckleRevitPlugin
{
    /// <summary>
    /// Interaction logic for form_Main.xaml
    /// </summary>
    public partial class form_Main
    {
        //private SettingsHelper _s;

        public form_Main()
        {
            InitializeComponent();
            Label_Version.Content = "v." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            //widen scope
            //_s = s;
        }

        #region Form Events
        /// <summary>
        /// Close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }
        #endregion
    }
}
