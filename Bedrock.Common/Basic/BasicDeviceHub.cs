using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bedrock.Common;

namespace Bedrock.Shared
{
    public class BasicDeviceHub : DeviceHub
    {
        public override IEnumerable<Service> Services
        {
            get
            {
                return services.ToArray();
            }
        }
        public override IEnumerable<Device> Devices
        {
            get
            {
                return devices.ToArray();
            }
        }

        private List<Service> services = new List<Service>();
        private List<Device> devices = new List<Device>();

        public BasicDeviceHub()
        {
        }

        public void AddService(Service service)
        {
            services.Add(service);
        }
        public void AddDevice(Device device)
        {
            devices.Add(device);
        }
    }
}