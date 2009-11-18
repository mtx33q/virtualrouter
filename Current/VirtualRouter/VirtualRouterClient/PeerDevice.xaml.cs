/*
* Virtual Router v0.8 Beta - http://virtualrouter.codeplex.com
* Wifi Hot Spot for Windows 7 and 2008 R2
* Copyright (c) 2009 Chris Pietschmann (http://pietschsoft.com)
* Licensed under the Microsoft Public License (Ms-PL)
* http://virtualrouter.codeplex.com/license
*/
using System;
using System.Threading;
using System.Windows.Controls;
using VirtualRouterClient.VirtualRouterService;
using System.Windows.Media;

namespace VirtualRouterClient
{
    /// <summary>
    /// Interaction logic for PeerDevice.xaml
    /// </summary>
    public partial class PeerDevice : UserControl
    {
        Thread thread;

        public PeerDevice(ConnectedPeer peer)
        {
            InitializeComponent();

            this.Peer = peer;
        }

        private ConnectedPeer _Peer;
        public ConnectedPeer Peer {
            get
            {
                return this._Peer;
            }
            private set
            {
                this._Peer = value;

                lblDisplayName.Content = lblDisplayName.ToolTip = this._Peer.MacAddress;

                ShowDeviceIcon();

                lblMACAddress.Content = "";
                lblIPAddress.Content = "";
                
                // TODO - Need to get IP Address
                lblMACAddress.Content = "Retrieving Host Name...";
                lblIPAddress.Content = "Retrieving IP Address...";

                thread = new Thread(new ParameterizedThreadStart(this.getIPInfo));
                thread.Start(this);
            }
        }

        public void ShowDeviceIcon()
        {   
            var icon = DeviceIconManager.LoadIcon(this._Peer.MacAddress);
            var resourceName = icon.Icon.ToResourceName();
            imgDeviceIcon.Source = (ImageSource)FindResource(resourceName);
        }

        public string IPAddress { get; set; }
        public string HostName { get; set; }

        private void SetIPInfoDisplay(IPInfo ipinfo)
        {
            if (ipinfo.HostName == ipinfo.IPAddress)
            {
                lblDisplayName.Content = lblDisplayName.ToolTip = ipinfo.HostName;
            }
            else
            {
                if (string.IsNullOrEmpty(ipinfo.HostName))
                {
                    lblDisplayName.Content = lblDisplayName.ToolTip = ipinfo.IPAddress;
                }
                else
                {
                    lblDisplayName.Content = lblDisplayName.ToolTip = ipinfo.HostName;
                }
            }

            this.lblMACAddress.Content = "MAC: " + ipinfo.MacAddress;
            this.lblIPAddress.Content = "IP: " + ipinfo.IPAddress;

            this.IPAddress = ipinfo.IPAddress;
            this.HostName = ipinfo.HostName;
        }

        private void getIPInfo(object data)
        {
            var ipinfoFound = false;
            while (!ipinfoFound)
            {
                try
                {
                    var pd = data as PeerDevice;
                    var ipinfo = IPInfo.GetIPInfo(pd.Peer.MacAddress.Replace(":", "-"));
                    if (ipinfo != null)
                    {
                        var hostname = ipinfo.HostName;
                        this.Dispatcher.Invoke((Action)delegate()
                        {
                            this.SetIPInfoDisplay(ipinfo);
                        });
                        ipinfoFound = true;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        ipinfoFound = false;
                    }
                }
                catch
                {
                    ipinfoFound = false;
                }
            }
        }

        private void UserControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var window = new PeerDeviceProperties(this);
            window.ShowDialog();
        }

    }
}
