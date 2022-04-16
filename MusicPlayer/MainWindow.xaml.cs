using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();

            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.Icon = new System.Drawing.Icon("icon.ico");
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick +=
                delegate(object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
        }

        protected void OnClosed(object? sender, EventArgs eventArgs)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void OnStateChanged(object? sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized){
                this.notifyIcon.ShowBalloonTip(5000, "Minimized", "Application will continue in tray mode",
                    ToolTipIcon.Info);
                this.Hide();
            }
        }
    }
}