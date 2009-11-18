/*
* Virtual Router v0.9 Beta - http://virtualrouter.codeplex.com
* Wifi Hot Spot for Windows 7 and 2008 R2
* Copyright (c) 2009 Chris Pietschmann (http://pietschsoft.com)
* Licensed under the Microsoft Public License (Ms-PL)
* http://virtualrouter.codeplex.com/license
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualRouter.Wlan;
using IcsMgr;
using System.ServiceModel;
using VirtualRouter.Wlan.WinAPI;

namespace VirtualRouterHost
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class VirtualRouterHost : IVirtualRouterHost
    {
        private WlanManager wlanManager;
        private IcsManager icsManager;

        private SharableConnection currentSharedConnection;

        public VirtualRouterHost()
        {
            this.wlanManager = new WlanManager();
            this.icsManager = new IcsManager();
        }

        #region IVirtualRouterHost Members

        public bool Start(SharableConnection sharedConnection)
        {
            try
            {
                if (this.icsManager.SharingInstalled)
                {
                    var privateConnectionGuid = this.wlanManager.HostedNetworkInterfaceGuid;

                    if (privateConnectionGuid == Guid.Empty)
                    {
                        // If the GUID for the Hosted Network Adapter isn't return properly,
                        // then retrieve it by the DeviceName.

                        privateConnectionGuid = (from c in this.icsManager.Connections
                                                 where c.props.DeviceName.ToLowerInvariant().Contains("microsoft virtual wifi miniport adapter")
                                                 select c.Guid).First();
                        // Note: This may not work correctly if there are multiple wireless adapters within the computer. I don't currently
                        // have a device to test this on. Ultimately, the "privateConnectionGuid" being Empty is what needs to be fixed.
                    }

                    this.icsManager.EnableIcs(sharedConnection.Guid, privateConnectionGuid);

                    this.currentSharedConnection = sharedConnection;
                }

                this.wlanManager.StartHostedNetwork();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                if (this.icsManager.SharingInstalled)
                {
                    this.icsManager.DisableIcsOnAll();
                }

                this.wlanManager.StopHostedNetwork();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SetConnectionSettings(string ssid, int maxNumberOfPeers)
        {
            try
            {
                this.wlanManager.SetConnectionSettings(ssid, maxNumberOfPeers);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ConnectionSettings GetConnectionSettings()
        {
            try
            {
                string ssid;
                int maxNumberOfPeers;

                var r = this.wlanManager.QueryConnectionSettings(out ssid, out maxNumberOfPeers);

                return new ConnectionSettings()
                {
                    SSID = ssid,
                    MaxPeerCount = maxNumberOfPeers
                };
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<SharableConnection> GetSharableConnections()
        {
            List<IcsConnection> connections;
            try
            {
                connections = this.icsManager.Connections;
            }
            catch
            {
                connections = new List<IcsConnection>();
            }

            if (connections != null)
            {
                foreach (var conn in connections)
                {
                    if (conn.IsConnected && conn.IsSupported)
                    {
                        yield return new SharableConnection(conn);
                    }
                }
            }
        }

        public bool SetPassword(string password)
        {
            try
            {
                this.wlanManager.SetSecondaryKey(password);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetPassword()
        {
            try
            {
                string passKey = string.Empty;
                bool isPassPhrase;
                bool isPersistent;

                var r = this.wlanManager.QuerySecondaryKey(out passKey, out isPassPhrase, out isPersistent);

                return passKey;
            }
            catch
            {
                return null;
            }
        }

        public bool IsStarted()
        {
            try
            {
                return wlanManager.IsHostedNetworkStarted;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<ConnectedPeer> GetConnectedPeers()
        {
            foreach (var v in wlanManager.Stations)
            {
                yield return new ConnectedPeer(v.Value);
            }
        }

        public SharableConnection GetSharedConnection()
        {
            return this.currentSharedConnection;
        }

        #endregion
    }
}
