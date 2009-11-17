using System.Threading;
/*
* Virtual Router v0.8 Beta - http://virtualrouter.codeplex.com
* Wifi Hot Spot for Windows 7 and 2008 R2
* Copyright (c) 2009 Chris Pietschmann (http://pietschsoft.com)
* Licensed under the Microsoft Public License (Ms-PL)
* http://virtualrouter.codeplex.com/license
*/
using System.Windows.Controls;
using VirtualRouterClient.VirtualRouterService;
using System;

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
                
                lblMACAddress.Content = "";
                lblIPAddress.Content = "";
                
                // TODO - Need to get IP Address
                lblMACAddress.Content = "Retrieving Host Name...";
                lblIPAddress.Content = "Retrieving IP Address...";

                thread = new Thread(new ParameterizedThreadStart(this.getIPInfo));
                thread.Start(this);
            }
        }

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
        }

        private void getIPInfo(object data)
        {
            var pd = (PeerDevice)data;
            var ipinfo = IPInfo.GetIPInfo(pd.Peer.MacAddress.Replace(":", "-"));
            var hostname = ipinfo.HostName;
            this.Dispatcher.Invoke((Action)delegate()
            {
                this.SetIPInfoDisplay(ipinfo);
            });
        }

    }
}
