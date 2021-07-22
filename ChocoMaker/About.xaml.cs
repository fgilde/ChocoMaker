using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Core;


namespace ChocoMaker
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : ThemedWindow
    {
        public About()
        {
            InitializeComponent();
        }
        
        private void About_OnClosing(object sender, CancelEventArgs e)
        {
            if (!((App)Application.Current).IsLoaded)
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            else if (AllowsTransparency)
            {
                Closing -= About_OnClosing;
                e.Cancel = true;
                var anim = new DoubleAnimation(0, TimeSpan.FromMilliseconds(600));
                anim.Completed += (s, _) => Close();
                BeginAnimation(OpacityProperty, anim);
            }
        }
    }
}
