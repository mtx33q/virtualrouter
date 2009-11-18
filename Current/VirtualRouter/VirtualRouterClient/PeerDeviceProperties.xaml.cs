/*
* Virtual Router v0.8 Beta - http://virtualrouter.codeplex.com
* Wifi Hot Spot for Windows 7 and 2008 R2
* Copyright (c) 2009 Chris Pietschmann (http://pietschsoft.com)
* Licensed under the Microsoft Public License (Ms-PL)
* http://virtualrouter.codeplex.com/license
*/
using System.Windows;
using VirtualRouterClient.AeroGlass;

namespace VirtualRouterClient
{
    /// <summary>
    /// Interaction logic for PeerDeviceProperties.xaml
    /// </summary>
    public partial class PeerDeviceProperties : Window
    {
        public PeerDeviceProperties(PeerDevice peerDevice)
        {
            this.PeerDevice = peerDevice;

            InitializeComponent();

            this.Loaded += new RoutedEventHandler(PeerDeviceProperties_Loaded);

            this.UpdateDisplay();
        }

        private void PeerDeviceProperties_Loaded(object sender, RoutedEventArgs e)
        {
            AeroGlassHelper.ExtendGlass(this, (int)windowContent.Margin.Left, (int)windowContent.Margin.Right, (int)windowContent.Margin.Top, (int)windowContent.Margin.Bottom);
        }

        public PeerDevice PeerDevice { get; private set; }

        private void UpdateDisplay()
        {
            if (this.PeerDevice != null)
            {
                this.Icon = this.imgDeviceIcon.Source = this.PeerDevice.imgDeviceIcon.Source;

                this.lblDisplayName.Content = this.lblDisplayName.ToolTip = this.Title = this.PeerDevice.lblDisplayName.Content.ToString();

                this.txtMACAddress.Text = this.PeerDevice.Peer.MacAddress;
                this.txtIPAddress.Text = this.PeerDevice.IPAddress;
                this.txtHostName.Text = this.PeerDevice.HostName;
            }
        }
    }
}
