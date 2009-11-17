using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

            this.UpdateDisplay();
        }

        public PeerDevice PeerDevice { get; private set; }

        private void UpdateDisplay()
        {
            if (this.PeerDevice != null)
            {
                this.imgDeviceIcon.Source = this.PeerDevice.imgDeviceIcon.Source;
                this.lblDisplayName.Content = this.lblDisplayName.ToolTip = this.Title = this.PeerDevice.lblDisplayName.Content.ToString();

                this.txtMACAddress.Text = this.PeerDevice.Peer.MacAddress;
                this.txtIPAddress.Text = this.PeerDevice.IPAddress;
                this.txtHostName.Text = this.PeerDevice.HostName;
            }
        }
    }
}
