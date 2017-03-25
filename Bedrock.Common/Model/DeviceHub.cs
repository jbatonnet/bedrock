using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Bedrock.Common
{
    public abstract class DeviceHub : Device
    {
        public virtual IEnumerable<Device> Devices { get; }

        public virtual Device FindDevice(params Guid[] path)
        {
            if (path == null || path.Length == 0)
                throw new IndexOutOfRangeException("The specified path is not valid");

            Device device = Devices.FirstOrDefault(d => d.Id == path[0]);
            if (device == null)
                throw new Exception("Could not find any device matching the secified path");
            if (path.Length == 1)
                return device;

            DeviceHub deviceHub = device as DeviceHub;
            if (deviceHub == null)
                throw new Exception("One element in the path is not a device hub");

            return deviceHub.FindDevice(path.Skip(1).ToArray());
        }
    }
}