using System.Windows;
using DevExpress.Xpf.Core;

namespace ChocoMaker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Window about;

        public bool IsLoaded { get; private set; }

        public App()
        {
            ApplicationThemeHelper.ApplicationThemeName = Theme.VS2017DarkName;  //Theme.Office2019BlackName;
            ShowSplash();
        }

        internal void CloseSplash()
        {
            IsLoaded = true;
            about?.Close();
            about = null;
        }

        private void ShowSplash()
        {
            about = new About {WindowStartupLocation = WindowStartupLocation.CenterScreen, WindowStyle = WindowStyle.None, AllowsTransparency = true};
            about.Show();
        }
    }
}
