﻿using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace Bedrock.Common
{
    public abstract class Service : MarshalByRefObject
    {
        public virtual Guid Id
        {
            get
            {
                if (id == null)
                {
                    NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                    NetworkInterface networkInterface = networkInterfaces.FirstOrDefault(i => i.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                    byte[] addressBytes = networkInterface.GetPhysicalAddress().GetAddressBytes(); // 48 bits: MAC
                    byte[] typeBytes = BitConverter.GetBytes(GetType().FullName.GetHashCode());    // 32 bits: Type
                    byte[] nameBytes = BitConverter.GetBytes(Name?.GetHashCode() ?? 0);            // 32 bits: Name
                                                                                                   // 16 bits: Other

                    byte[] guidBytes = new byte[16];
                    int offset = 0;

                    Array.Copy(addressBytes, 0, guidBytes, offset, addressBytes.Length); offset += addressBytes.Length;
                    Array.Copy(typeBytes, 0, guidBytes, offset, typeBytes.Length); offset += typeBytes.Length;
                    Array.Copy(nameBytes, 0, guidBytes, offset, nameBytes.Length); offset += nameBytes.Length;

                    id = new Guid(guidBytes);
                }

                return id.Value;
            }
        }
        public virtual string Name { get; }
        public virtual string Description { get; }

        private Guid? id;

        public override object InitializeLifetimeService()
        {
            return null;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}