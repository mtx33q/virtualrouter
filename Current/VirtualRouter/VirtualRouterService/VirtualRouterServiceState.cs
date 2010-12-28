﻿/*
* Virtual Router v0.8 Beta - http://virtualrouter.codeplex.com
* Wifi Hot Spot for Windows 7 and 2008 R2
* Copyright (c) 2011 Chris Pietschmann (http://pietschsoft.com)
* Licensed under the Microsoft Public License (Ms-PL)
* http://virtualrouter.codeplex.com/license
*/
using System.Runtime.Serialization;

namespace VirtualRouterService
{
    [DataContract]
    public class VirtualRouterServiceState
    {
        [DataMember]
        public bool IsStarted { get; set; }
        [DataMember]
        public string SharedConnectionGuid { get; set; }
        [DataMember]
        public string SSID { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public int MaxPeerCount { get; set; }
    }
}
