using System;
using System.ComponentModel;

namespace VirtualRouterClient
{
    public enum DeviceIconEnum : int
    {
        [Description("Default"), DeviceIconResourceName("IconDefault")]
        Default = 0,
        [Description("Computer"), DeviceIconResourceName("IconComputer")]
        Computer = 1,
        [Description("Device"), DeviceIconResourceName("IconDevice")]
        Device = 2,
        [Description("iPhone"), DeviceIconResourceName("IconIPhone")]
        IPhone = 3,
        [Description("Mac Book Pro"), DeviceIconResourceName("IconMacBookPro")]
        MacBookPro = 4,
        [Description("Printer"), DeviceIconResourceName("IconPrinter")]
        Printer = 5,
        [Description("Mobile Phone"), DeviceIconResourceName("IconMobilePhone")]
        MobilePhone = 6,
        [Description("Zune"), DeviceIconResourceName("IconZune")]
        Zune = 7,
        [Description("Camera"), DeviceIconResourceName("IconCamera")]
        Camera = 8
    }

    public static class DeviceIconEnumExtensions
    {
        public static string ToResourceName(this DeviceIconEnum val)
        {
            DeviceIconResourceNameAttribute[] attributes = (DeviceIconResourceNameAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DeviceIconResourceNameAttribute), false);
            return attributes.Length > 0 ? attributes[0].ResourceName : string.Empty;
        }

        public static string ToDescriptionString(this DeviceIconEnum val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }

    public class DeviceIconResourceNameAttribute : Attribute
    {
        public DeviceIconResourceNameAttribute(string resourceName)
        {
            this.ResourceName = resourceName;
        }

        public string ResourceName { get; private set; }
    }
}
