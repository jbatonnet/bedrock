using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

using Bedrock.Common;
using Bedrock.Shared;

namespace Bedrock.Shared
{
    [TypeConverter("Bedrock.Shared." + nameof(ServiceInfoConverter))]
    public class ServiceInfo
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Type Type { get; set; }
        public bool Favorite { get; set; }

        public DeviceInfo Device { get; set; }
        public Service Service { get; set; }

        public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();

        public ServiceInfo(DeviceInfo device, Guid id)
        {
            Id = id;
            Device = device;
        }
        public ServiceInfo(DeviceInfo device, Guid id, string name, string description = null, bool favorite = false, Type type = null)
        {
            Id = id;
            Name = name;
            Description = description;
            Device = device;
            Favorite = favorite;
            Type = type;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ServiceInfoConverter : TypeConverter
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
                //ServiceInfo serviceInfo = DeviceManager.FindServiceById((Guid)value);
                //return serviceInfo;
            }

            return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            ServiceInfo serviceInfo = value as ServiceInfo;
            if (serviceInfo == null)
                return null;

            if (destinationType == typeof(string))
                return serviceInfo.Id.ToString();
            else if (destinationType == typeof(Guid))
                return serviceInfo.Id;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}