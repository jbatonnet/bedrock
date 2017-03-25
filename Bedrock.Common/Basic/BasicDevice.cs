using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bedrock.Common;

namespace Bedrock.Shared
{
    public class BasicDeviceWrapper : Device
    {
        public override IEnumerable<Service> Services => services;

        protected IEnumerable<Service> services = Enumerable.Empty<Service>();
    }

    public class BasicDevice : BasicDeviceWrapper
    {
        public new IEnumerable<Service> Services
        {
            get
            {
                return services;
            }
            set
            {
                services = value;
            }
        }
    }
}