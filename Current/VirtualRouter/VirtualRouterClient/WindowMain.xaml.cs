/*
* Virtual Router v0.8 Beta - http://virtualrouter.codeplex.com
* Wifi Hot Spot for Windows 7 and 2008 R2
* Copyright (c) 2009 Chris Pietschmann (http://pietschsoft.com)
* Licensed under the Microsoft Public License (Ms-PL)
* http://virtualrouter.codeplex.com/license
*/
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using VirtualRouterClient.VirtualRouterService;
using System.Diagnostics;

namespace VirtualRouterClient
{
    /// <summary>
    /// Interaction logic for WindowMain.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        private App myApp = (App)App.Current;
        private Thread threadUpdateUI;

        private WpfNotifyIcon trayIcon;

        public WindowMain()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(Window1_Loaded);

            myApp.VirtualRouterServiceConnected += new EventHandler(myApp_VirtualRouterServiceConnected);
            myApp.VirtualRouterServiceDisconnected += new EventHandler(myApp_VirtualRouterServiceDisconnected);
        }

        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            var args = System.Environment.GetCommandLineArgs();
            var minarg = (from a in args
                          where a.ToLowerInvariant().Contains("/min")
                          select a).FirstOrDefault();
            if (!string.IsNullOrEmpty(minarg))
            {
                this.WindowState = WindowState.Minimized;
                this.ShowInTaskbar = false;
            }

            this.AddSystemMenuItems();

            this.threadUpdateUI = new Thread(new ThreadStart(this.UpdateUIThread));
            this.threadUpdateUI.Start();

            this.Closed += new EventHandler(Window1_Closed);


            // Show System Tray Icon
            var stream = Application.GetResourceStream(new Uri("icons/virtualrouterdisabled.ico", UriKind.Relative)).Stream;
            var icon = new System.Drawing.Icon(stream);
            this.trayIcon = new WpfNotifyIcon();
            this.trayIcon.Icon = icon;
            this.trayIcon.Show();
            this.trayIcon.Text = "Virtual Router (Disabled)";
            this.trayIcon.DoubleClick += new EventHandler(trayIcon_DoubleClick);

            var trayMenu = new System.Windows.Forms.ContextMenuStrip();
            trayMenu.Items.Add("&Manage Virtual Router...", null, new EventHandler(this.TrayIcon_Menu_Manage));
            trayMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            trayMenu.Items.Add("Check for &Updates...", null, new EventHandler(this.TrayIcon_Menu_Update));
            trayMenu.Items.Add("&About...", null, new EventHandler(this.TrayIcon_Menu_About));
            this.trayIcon.ContextMenuStrip = trayMenu;

            this.StateChanged += new EventHandler(WindowMain_StateChanged);

            UpdateDisplay();
        }

        void TrayIcon_Menu_Update(object sender, EventArgs e)
        {
            CheckUpdates();
        }

        public static void CheckUpdates()
        {
            Process.Start("http://virtualrouter.codeplex.com");
        }

        void TrayIcon_Menu_About(object sender, EventArgs e)
        {
            ShowAboutBox();
        }

        void TrayIcon_Menu_Manage(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        void WindowMain_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
            else
            {
                this.ShowInTaskbar = true;
            }
        }

        void trayIcon_DoubleClick(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void Window1_Closed(object sender, EventArgs e)
        {
            this.threadUpdateUI.Abort();
            this.trayIcon.Hide();
            this.trayIcon.Dispose();
        }

        void UpdateUIThread()
        {
            while (true)
            {
                this.Dispatcher.Invoke(new Action(this.UpdateDisplay));
                Thread.Sleep(5000); // 5 Seconds
            }
        }

        void myApp_VirtualRouterServiceDisconnected(object sender, EventArgs e)
        {
            lblStatus.Content = "Can not manage Virtual Router. The Service is not running.";
            this.trayIcon.Text = "Virtual Router (Disabled)";
            UpdateDisplay();
        }

        void myApp_VirtualRouterServiceConnected(object sender, EventArgs e)
        {
            lblStatus.Content = "Virtual Router can now be managed.";
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            UpdateUIDisplay();

            RefreshSharableConnectionsDisplay();

            if (myApp.IsVirtualRouterServiceConnected)
            {
                panelConnections.Children.Clear();
                var peers = myApp.VirtualRouter.GetConnectedPeers();
                groupBoxPeersConnected.Header = "Peers Connected (" + peers.Count().ToString() + "):";
                foreach (var p in peers)
                {
                    panelConnections.Children.Add(new PeerDevice(p));
                }
            }
            else
            {
                groupBoxPeersConnected.Header = "Peers Connected (0):";
            }
        }

        #region "System Menu Stuff"

        #region Win32 API Stuff

        // Define the Win32 API methods we are going to use
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIDNewItem, string lpNewItem);

        /// Define our Constants we will use
        public const Int32 WM_SYSCOMMAND = 0x112;
        public const Int32 MF_SEPARATOR = 0x800;
        public const Int32 MF_BYPOSITION = 0x400;
        public const Int32 MF_STRING = 0x0;

        #endregion 

        private const int _AboutSysMenuID = 1001;
        private const int _UpdateSysMenuID = 1002;

        private void AddSystemMenuItems()
        {
            IntPtr windowHandle = new WindowInteropHelper(this).Handle;
            IntPtr systemMenu = GetSystemMenu(windowHandle, false);

            InsertMenu(systemMenu, 5, MF_BYPOSITION | MF_SEPARATOR, 0, string.Empty);
            InsertMenu(systemMenu, 6, MF_BYPOSITION, _UpdateSysMenuID, "Check for Updates...");
            InsertMenu(systemMenu, 7, MF_BYPOSITION, _AboutSysMenuID, "About...");

            HwndSource source = HwndSource.FromHwnd(windowHandle);
            source.AddHook(new HwndSourceHook(WndProc));
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Check if a System Command has been executed
            if (msg == WM_SYSCOMMAND)
            {
                // Execute the appropriate code for the System Menu item that was clicked
                switch (wParam.ToInt32())
                {
                    case _AboutSysMenuID:
                        ShowAboutBox();
                        handled = true;
                        break;
                    case _UpdateSysMenuID:
                        CheckUpdates();
                        handled = true;
                        break;
                }
            }

            return IntPtr.Zero;
        }

        #endregion

        static void ShowAboutBox()
        {
            System.Windows.MessageBox.Show(
                            AssemblyAttributes.AssemblyProduct + " " + AssemblyAttributes.AssemblyVersion + Environment.NewLine
                            + Environment.NewLine + AssemblyAttributes.AssemblyDescription + Environment.NewLine
                            + Environment.NewLine + "Licensed under the Microsoft Public License (Ms-PL)" + Environment.NewLine
                            + Environment.NewLine + AssemblyAttributes.AssemblyCopyright + Environment.NewLine
                            + Environment.NewLine + "http://virtualrouter.codeplex.com"
                            
                            , "About " + AssemblyAttributes.AssemblyProduct + "...");
        }

        private void btnToggleHostedNetwork_Click(object sender, RoutedEventArgs e)
        {
            if (myApp.IsVirtualRouterServiceConnected)
            {
                if (myApp.VirtualRouter.IsStarted())
                {
                    myApp.VirtualRouter.Stop();
                }
                else
                {

                    myApp.VirtualRouter.SetConnectionSettings(txtSSID.Text, 100);
                    myApp.VirtualRouter.SetPassword(txtPassword.Text);

                    if (!myApp.VirtualRouter.Start((SharableConnection)cbSharedConnection.SelectedItem))
                    {
                        string strMessage = "Virtual Router Could Not Be Started!";
                        lblStatus.Content = strMessage;
                        MessageBox.Show(strMessage, this.Title);
                    }
                    else
                    {
                        lblStatus.Content = "Virtual Router Started...";
                    }
                }
            }

            UpdateUIDisplay();
        }

        private bool isVirtualRouterRunning = false;
        private void UpdateUIDisplay()
        {
            var enableToggleButton = false;
            var enableSettingsFields = false;

            if (myApp.IsVirtualRouterServiceConnected)
            {
                enableToggleButton = true;
                try
                {
                    btnToggleHostedNetwork.IsEnabled = true;
                    if (myApp.VirtualRouter.IsStarted())
                    {
                        enableSettingsFields = false;
                        btnToggleHostedNetwork.Content = "Stop Virtual Router";
                        this.trayIcon.Text = "Virtual Router (Running)";
                        this.trayIcon.Icon = new System.Drawing.Icon(
                            Application.GetResourceStream(new Uri("icons/virtualrouterenabled.ico", UriKind.Relative)).Stream
                            );

                        if (!isVirtualRouterRunning)
                        {
                            this.Icon = BitmapFrame.Create(Application.GetResourceStream(new Uri("Icons/VirtualRouterEnabled.ico", UriKind.Relative)).Stream);
                        }
                        isVirtualRouterRunning = true;

                        txtSSID.Text = myApp.VirtualRouter.GetConnectionSettings().SSID;
                        txtPassword.Text = myApp.VirtualRouter.GetPassword();
                    }
                    else
                    {
                        enableSettingsFields = true;
                        btnToggleHostedNetwork.Content = "Start Virtual Router";
                        this.trayIcon.Text = "Virtual Router (Stopped)";
                        this.trayIcon.Icon = new System.Drawing.Icon(
                            Application.GetResourceStream(new Uri("icons/virtualrouterdisabled.ico", UriKind.Relative)).Stream
                            );

                        if (isVirtualRouterRunning)
                        {
                            this.Icon = BitmapFrame.Create(Application.GetResourceStream(new Uri("Icons/VirtualRouterDisabled.ico", UriKind.Relative)).Stream);
                        }
                        isVirtualRouterRunning = false;
                    }
                }
                catch
                {
                    enableToggleButton = false;
                    enableSettingsFields = false;
                }
            }

            btnToggleHostedNetwork.IsEnabled = enableToggleButton;
            gbVirtualRouterSettings.IsEnabled = enableSettingsFields;
        }

        private void RefreshSharableConnectionsDisplay()
        {
            if (myApp.IsVirtualRouterServiceConnected)
            {
                SharableConnection previousItem = cbSharedConnection.SelectedItem as SharableConnection;

                cbSharedConnection.Items.Clear();
                cbSharedConnection.DisplayMemberPath = "Name";
                var connections = myApp.VirtualRouter.GetSharableConnections();
                foreach (var c in connections)
                {
                    cbSharedConnection.Items.Add(c);
                }

                if (previousItem == null)
                {
                    cbSharedConnection.SelectedIndex = 0;
                }
                else
                {
                    var newSelectIndex = 0;
                    for (var i = 0; i < cbSharedConnection.Items.Count - 1; i++)
                    {
                        if (previousItem.Guid.ToString() == ((SharableConnection)cbSharedConnection.Items[i]).Guid.ToString())
                        {
                            newSelectIndex = i;
                            break;
                        }
                    }
                    cbSharedConnection.SelectedIndex = 0;
                }
            }
        }

        private void btnRefreshSharableConnections_Click(object sender, RoutedEventArgs e)
        {
            RefreshSharableConnectionsDisplay();
        }
    }
}
