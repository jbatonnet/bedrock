using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Bedrock.Common;

namespace Bedrock.Shared
{
    /// <summary>
    /// Describes a Bedrock device
    /// </summary>
    [TypeConverter("Bedrock.Shared." + nameof(DeviceInfoConverter))]
    public class DeviceInfo
    {
        private static Dictionary<Guid, DeviceInfo> devicesInfo { get; } = new Dictionary<Guid, DeviceInfo>();

        public event EventHandler Updated;

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Hub { get; set; }

        public Device Device { get; private set; }

        public IEnumerable<ServiceInfo> Services
        {
            get
            {
                return services.AsReadOnly();
            }
            set
            {
                services = value as List<ServiceInfo> ?? value.ToList();
            }
        }
        public IEnumerable<DeviceInfo> Devices
        {
            get
            {
                return devices.AsReadOnly();
            }
            set
            {
                devices = value as List<DeviceInfo> ?? value.ToList();
            }
        }

        private bool deviceProxy;

        private List<ServiceInfo> services = new List<ServiceInfo>();
        private List<DeviceInfo> devices = new List<DeviceInfo>();

        private DeviceInfo(Device device)
        {
            Id = device.Id;
            Hub = device is DeviceHub;
            Device = device;

            deviceProxy = false;
        }
        private DeviceInfo(Guid id)
        {
            Id = id;

            deviceProxy = true;
        }
        public DeviceInfo(Guid id, string name, string description, bool hub)
        {
            Id = id;
            Name = name;
            Description = description;
            Hub = hub;
        }

        public static DeviceInfo Get(Device device)
        {
            DeviceInfo info;
            Guid id = device.Id;

            if (!devicesInfo.TryGetValue(id, out info))
            {
                info = new DeviceInfo(device);
                devicesInfo.Add(id, info);
            }

            return info;
        }
        public static DeviceInfo Get(Guid id)
        {
            DeviceInfo device;

            if (devicesInfo.TryGetValue(id, out device))
                return device;
            else
                return null;
        }

        public async Task Update(IEnumerable<PathInfo> paths = null)
        {
            await Task.Run(() =>
            {
                // General info
                Id = Device.Id;
                Name = Device.Name;
                Description = Device.Description;

                // Sub devices
                devices.Clear();

                DeviceHub deviceHub = Device as DeviceHub;
                if (deviceHub != null)
                {
                    foreach (Device device in deviceHub.Devices)
                        devices.Add(DeviceInfo.Get(device));
                }

                // Services
                /*services.Clear();
                
                foreach (Service service in Device.Services)
                    services.Add(ServiceInfo.Get(service));*/

                Updated?.Invoke(this, EventArgs.Empty);
            });
        }
        public void UpdatePaths(List<PathInfo> paths, bool reconnectIfNeeded = false)
        {
            if (!deviceProxy)
                throw new NotSupportedException("Cannot update path of non proxy devices");


        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class DeviceInfoConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else if (sourceType == typeof(Guid))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                Guid guid;
                if (!Guid.TryParse((string)value, out guid))
                    return null;

                value = guid;
            }

            if (value is Guid)
            {
                DeviceInfo deviceInfo = DeviceInfo.Get((Guid)value);
                return deviceInfo;
            }

            return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!(value is DeviceInfo))
                return null;

            if (destinationType == typeof(string))
                return (value as DeviceInfo).Id.ToString();
            else if (destinationType == typeof(Guid))
                return (value as DeviceInfo).Id;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}