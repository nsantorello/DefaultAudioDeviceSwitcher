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
    public class AudioDevice
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public bool IsDefault { get; private set; }

        public AudioDevice(IntPtr pointer)
        {
            var device = (AudioDeviceStruct)Marshal.PtrToStructure(pointer, typeof(AudioDeviceStruct));
            Id = Marshal.PtrToStringUni(device.id);
            Name = Marshal.PtrToStringUni(device.name);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AudioDeviceStruct
    {
        public IntPtr id;
        public IntPtr name;
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private NotifyIcon _icon;
        private System.Windows.Controls.ContextMenu _contextMenu;
        private IEnumerable<AudioDevice> _deviceNames = GetDevices();
        private IEnumerable<System.Windows.Controls.MenuItem> _menuItems = new List<System.Windows.Controls.MenuItem>();

        [DllImport("EndPointController.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetDevices")]
        private static extern void GetDevicesNative(IntPtr[] devices);

        [DllImport("EndPointController.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern bool SetDefaultDevice(string deviceName);

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _contextMenu = (System.Windows.Controls.ContextMenu)this.FindResource("_notifierContextMenu");
            _contextMenu.Items.Clear();
            foreach (var device in _deviceNames)
            {
                var thisDevice = device;
                var menuItem = new System.Windows.Controls.MenuItem { Header = thisDevice.Name };
                menuItem.Click += (a, b) => SetDefaultDevice(thisDevice.Id);
                _contextMenu.Items.Add(menuItem);
            }
            
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
            _contextMenu.IsOpen = true;
        }

        private static IEnumerable<AudioDevice> GetDevices()
        {
            var devices = new List<AudioDevice>();
            var pointers = new IntPtr[100];
            GetDevicesNative(pointers);

            foreach (var ptr in pointers)
            {
                if (ptr == IntPtr.Zero) break;
                devices.Add(new AudioDevice(ptr));
            }

            return devices;
        }
    }
}
