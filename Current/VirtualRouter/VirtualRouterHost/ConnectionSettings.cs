/*
* Virtual Router v0.9 Beta - http://virtualrouter.codeplex.com
* Wifi Hot Spot for Windows 7 and 2008 R2
* Copyright (c) 2009 Chris Pietschmann (http://pietschsoft.com)
* Licensed under the Microsoft Public License (Ms-PL)
* http://virtualrouter.codeplex.com/license
*/
using System.Runtime.Serialization;

namespace VirtualRouterHost
{
    [DataContract]
    public class ConnectionSettings
    {
        [DataMember]
        public string SSID { get; set; }
        [DataMember]
        public int MaxPeerCount { get; set; }
    }
}
