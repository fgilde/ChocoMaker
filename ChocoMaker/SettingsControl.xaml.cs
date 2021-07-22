using System.Windows;
using System.Windows.Controls;

namespace ChocoMaker
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
            DataContext = Properties.Settings.Default;
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
        }

        private void ButtonInfo_OnClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
            DataContext = null;
            DataContext = Properties.Settings.Default;
        }
    }
}
