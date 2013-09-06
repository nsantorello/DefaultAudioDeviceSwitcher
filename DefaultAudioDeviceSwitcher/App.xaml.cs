using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DefaultAudioDeviceSwitcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private NotifyIcon _icon;
        private System.Windows.Controls.ContextMenu _contextMenu;
        private IEnumerable<string> _deviceNames = GetDevices();

        [DllImport("EndPointController.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetDevices")]
        private static extern void GetDevicesNative(IntPtr[] deviceNames);

        //private static extern bool SetDefaultDevice(string deviceName);

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _contextMenu = (System.Windows.Controls.ContextMenu)this.FindResource("_notifierContextMenu");
            _contextMenu.DataContext = ...

            _icon = new NotifyIcon
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Text = "Default Audio Device Switcher",
                Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location),
                Visible = true
            };
            _icon.MouseClick += IconClicked;
            _icon.MouseDoubleClick += IconClicked;
        }

        private void IconClicked(object sender, EventArgs e)
        {
            menu.IsOpen = true;
        }

        private static IEnumerable<string> GetDevices()
        {
            var deviceNames = new List<string>();
            var pointers = new IntPtr[100];
            GetDevicesNative(pointers);

            foreach (var ptr in pointers)
            {
                if (ptr == IntPtr.Zero) break;
                deviceNames.Add(Marshal.PtrToStringUni(ptr));
            }

            return deviceNames;
        }
    }
}
