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
using VirtualRouterClient.Properties;

namespace VirtualRouterClient
{
    public static class DeviceIconManager
    {
        public static DeviceIcon LoadIcon(string macAddress)
        {
            if (Settings.Default.DeviceIcons == null)
            {
                Settings.Default.DeviceIcons = new System.Collections.ArrayList();
            }

            //retrieve setting
            DeviceIcon di = (from obj in Settings.Default.DeviceIcons.ToArray()
                             where (obj as DeviceIcon).MacAddress.Replace(":", "-").ToLowerInvariant() == macAddress.Replace(":", "-").ToLowerInvariant()
                             select obj as DeviceIcon).FirstOrDefault();
            if (di == null)
                return new DeviceIcon(macAddress, DeviceIconEnum.Default);
            else
                return di;
        }

        public static void SaveIcon(string macAddress, DeviceIconEnum icon)
        {
            if (Settings.Default.DeviceIcons == null)
            {
                Settings.Default.DeviceIcons = new System.Collections.ArrayList();

            }
            // Remove existing setting
            Settings.Default.DeviceIcons.Remove(
                    (from obj in Settings.Default.DeviceIcons.ToArray()
                     where (obj as DeviceIcon).MacAddress.Replace(":", "-").ToLowerInvariant() == macAddress.Replace(":", "-").ToLowerInvariant()
                         select obj).FirstOrDefault()
                );

            // Save new setting
            Settings.Default.DeviceIcons.Add(new DeviceIcon(macAddress, icon));
            Settings.Default.Save();
        }
    }

    public class DeviceIcon
    {
        public DeviceIcon(string macAddress, DeviceIconEnum icon)
        {
            this.MacAddress = macAddress;
            this.Icon = icon;
        }

        public string MacAddress { get; set; }
        public DeviceIconEnum Icon { get; set; }
    }
}
