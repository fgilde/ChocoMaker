using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using DevExpress.Xpf.WindowsUI;
using Microsoft.Win32;

namespace ChocoMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region P Invoke

        // P/Invoke constants
        private const int WM_SYSCOMMAND = 0x112;
        private const int MF_STRING = 0x0;
        private const int MF_SEPARATOR = 0x800;

        // P/Invoke declarations
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InsertMenu(IntPtr hMenu, int uPosition, int uFlags, int uIDNewItem, string lpNewItem);

        // ID for the About item on the system menu
        private int SYSMENU_ABOUT_ID = 0x1;
        private int SYSMENU_SETTINGS_ID = 0x2;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SaveToFileClick(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { FileName = "Choco Install.bat", Filter = "Bat Script|*.bat;*.cmd;" };
            if (dlg.ShowDialog() ?? false)
            {
                File.WriteAllText(dlg.FileName, scriptEdit.Text);
                ShowInformation("Saved successfully", $"File is saved to {dlg.FileName}");
            }
        }

        private void CopyToClipboardClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(scriptEdit.Text);
            ShowInformation("Copied successfully", "Content is saved to clipboard !");
        }

        private static void ShowInformation(string title, string content)
        {
            new WinUIDialogWindow(title, MessageBoxButton.OK)
            { BorderBrush = new SolidColorBrush(Colors.DodgerBlue), BorderThickness = new Thickness(0, 1, 0, 1), Content = new TextBlock { Text = content } }.ShowDialog();
        }

        private void MainWindow_OnContentRendered(object sender, EventArgs e)
        {
            ((App)Application.Current).CloseSplash();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = (HwndSource)PresentationSource.FromVisual(this);
            source?.AddHook(WndProc);

            var helper = new WindowInteropHelper(this);
            IntPtr hSysMenu = GetSystemMenu(helper.Handle, false);

            InsertMenu(hSysMenu, 0x0 - 1, MF_SEPARATOR, 0, string.Empty);
            InsertMenu(hSysMenu, 0x0, MF_STRING, SYSMENU_ABOUT_ID, "&About…");
            InsertMenu(hSysMenu, 0x0, MF_STRING, SYSMENU_SETTINGS_ID, "&Settings…");

        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if ((msg == WM_SYSCOMMAND) && (int)wParam == SYSMENU_ABOUT_ID)
            {
                var about = new About { Width = Width, WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this };
                about.ShowDialog(MessageBoxButton.OK);
            }
            if ((msg == WM_SYSCOMMAND) && (int)wParam == SYSMENU_SETTINGS_ID)
            {
                var settingsControl = new SettingsControl();
                var save = new WinUIDialogWindow("Settings", MessageBoxButton.OKCancel)
                { BorderBrush = new SolidColorBrush(Colors.DodgerBlue),
                    BorderThickness = new Thickness(0, 1, 0, 1), Content = settingsControl }.ShowDialog();
                if (save ?? false)
                {
                    settingsControl.Save();
                }
            }

            return IntPtr.Zero;
        }

    }
}
