﻿using System;
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
using VirtualRouterClient.AeroGlass;

namespace VirtualRouterClient
{
    /// <summary>
    /// Interaction logic for DeviceIconPicker.xaml
    /// </summary>
    public partial class DeviceIconPicker : Window
    {
        public DeviceIconPicker()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(DeviceIconPicker_Loaded);


            var icons = new List<Icon>();

            var iconNames = Enum.GetNames(typeof(DeviceIconEnum));
            foreach (var n in iconNames)
            {
                var e = (DeviceIconEnum)Enum.Parse(typeof(DeviceIconEnum), n);
                icons.Add(new Icon() { Value = e, IconName = e.ToDescriptionString(), IconPath = e.ToResourceName() });
            }

            listIcons.ItemsSource = icons;
        }

        void DeviceIconPicker_Loaded(object sender, RoutedEventArgs e)
        {
            AeroGlassHelper.ExtendGlass(this, (int)windowContent.Margin.Left, (int)windowContent.Margin.Right, (int)windowContent.Margin.Top, (int)windowContent.Margin.Bottom);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        public class Icon
        {
            public string IconPath { get; set; }
            public string IconName { get; set; }
            public DeviceIconEnum Value { get; set; }
        }
    }
}
