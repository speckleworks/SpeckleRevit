using MaterialDesignThemes.Wpf;
using MaterialDesignColors;
using MaterialDesignThemes.MahApps;
using MahApps.Metro;
using MahApps.Metro.Controls;

namespace SpeckleRevitPlugin.Tools.WallTool
{
    /// <summary>
    /// Interaction logic for SpeckleWallView.xaml
    /// </summary>
    public partial class SpeckleWallView : MetroWindow
    {
        public SpeckleWallView()
        {
            InitializeComponent();

            //TODO: How do we force these assemblies to be resolved properly and loaded properly?
            //TODO: We can get an installer and copy these to locations that would enforce this.
            // (Konrad) We need to make sure that MaterialDesignThemes and MahApps
            // are actually loaded into the context. These DLLs are missing if they are not explicitly
            // loaded into the app via using statement.
            var unused1 = typeof(FlyoutAssist);
            var unused2 = typeof(ThemeManager);
            var unused3 = typeof(Hue);
            var unused4 = typeof(ShadowAssist);
        }
    }
}
