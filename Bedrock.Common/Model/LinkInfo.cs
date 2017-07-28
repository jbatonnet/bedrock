using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Bedrock.Common;

namespace Bedrock.Shared
{
    public abstract class PathNodeInfo
    {
        public DeviceInfo Destination { get; protected set; }
    }
    public class PathInfo : IEnumerable<PathNodeInfo>
    {
        public int Count => links.Count;
        public bool IsReadOnly => false;

        private List<PathNodeInfo> links;

        public PathInfo()
        {
            links = new List<PathNodeInfo>();
        }
        public PathInfo(PathNodeInfo link)
        {
            links = new List<PathNodeInfo>() { link };
        }
        public PathInfo(IEnumerable<PathNodeInfo> links)
        {
            this.links = links.ToList();
        }
        public PathInfo(IEnumerable<PathNodeInfo> links, PathNodeInfo link)
        {
            this.links = links.Concat(new[] { link }).ToList();
        }

        public static IEnumerable<KeyValuePair<DeviceInfo, List<PathInfo>>> BuildPaths(IEnumerable<DeviceInfo> devices, IEnumerable<LinkInfo> connections)
        {
            Dictionary<DeviceInfo, List<PathInfo>> knownDevices = devices.ToDictionary(d => d, d => null as List<PathInfo>);
            int initialCount = knownDevices.Count;

            while (true)
            {
                LinkInfo[] targetConnections = connections.Where(c => knownDevices.ContainsKey(c.Source) && !knownDevices.ContainsKey(c.Destination)).ToArray();
                if (targetConnections.Length == 0)
                    break;

                foreach (LinkInfo connection in targetConnections)
                {
                    List<PathInfo> sourcePaths = knownDevices[connection.Source];

                    if (sourcePaths == null)
                    {
                        PathInfo destinationPath = new PathInfo(connection);

                        List<PathInfo> destinationPaths;
                        if (!knownDevices.TryGetValue(connection.Destination, out destinationPaths))
                            knownDevices.Add(connection.Destination, destinationPaths = new List<PathInfo>());

                        destinationPaths?.Add(destinationPath);
                    }
                    else
                    {
                        foreach (PathInfo sourcePath in sourcePaths)
                        {
                            PathInfo destinationPath = new PathInfo(sourcePath, connection);

                            List<PathInfo> destinationPaths;
                            if (!knownDevices.TryGetValue(connection.Destination, out destinationPaths))
                                knownDevices.Add(connection.Destination, destinationPaths = new List<PathInfo>());

                            destinationPaths?.Add(destinationPath);
                        }
                    }
                }
            }

            return knownDevices.Skip(initialCount);

            /*
            // Clear device links
            foreach (DeviceInfo device in devices)
                device.Paths.Clear();

            // Compute neighbor devices
            foreach (NeighborInfo neighbor in neighbors)
                neighbor.Destination.Paths.Add(new LinkInfo[] { neighbor });

            // Find device paths
            List<KeyValuePair<DeviceInfo, LinkInfo[]>> devicesPaths = new List<KeyValuePair<DeviceInfo, LinkInfo[]>>();
            List<DeviceInfo> knownDevices = devices.Where(d => d.Paths.Count > 0).ToList();
            List<DeviceInfo> unknownDevices = devices.Except(knownDevices).ToList();

            // Register sub-devices
            foreach (DeviceInfo device in knownDevices)
            {
                device.devicesInfo.Clear();

                ConnectionInfo[] deviceConnections = connections.Where(c => c.Source == device).ToArray();
                foreach (ConnectionInfo connection in deviceConnections)
                    device.devicesInfo.Add(connection.Destination);
            }

            // Build every paths
            while (unknownDevices.Count > 0)
            {
                List<DeviceInfo> passDevices = new List<DeviceInfo>();

                foreach (DeviceInfo device in knownDevices)
                {
                    ConnectionInfo[] deviceConnections = connections.Where(c => c.Source == device && unknownDevices.Contains(c.Destination)).ToArray();

                    foreach (ConnectionInfo connection in deviceConnections)
                    {
                        List<LinkInfo[]> devicePaths = Enumerable.Concat(devicesPaths.Where(p => p.Key == device).Select(p => p.Value),
                                                                         neighbors.Where(n => n.Destination == device).Select(n => new LinkInfo[] { n }))
                                                                 .ToList();

                        if (devicePaths.Count == 0)
                            devicePaths.Add(new LinkInfo[0]);

                        foreach (LinkInfo[] devicePath in devicePaths)
                            devicesPaths.Add(new KeyValuePair<DeviceInfo, LinkInfo[]>(connection.Destination, devicePath.Concat(new[] { connection }).ToArray()));

                        passDevices.Add(connection.Destination);
                    }
                }

                if (passDevices.Count == 0)
                    break; // FIXME: Something wrong happenned

                foreach (DeviceInfo device in passDevices.Distinct())
                {
                    knownDevices.Add(device);
                    unknownDevices.Remove(device);
                }
            }

            // Save paths
            foreach (var devicePath in devicesPaths)
                devicePath.Key.Paths.Add(devicePath.Value);*/
        }

        public IEnumerator<PathNodeInfo> GetEnumerator() => links.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Describes a connection to a Bedrock server
    /// </summary>
    public class ConnectionInfo : PathNodeInfo
    {
        public string Method { get; }
        public string Address { get; }

        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        public ConnectionInfo(DeviceInfo destination, string method, string address)
        {
            Destination = destination;

            Method = method;
            Address = address;
        }
        public ConnectionInfo(DeviceInfo destination, string method, string address, object parameters)
        {
            Destination = destination;

            Method = method;
            Address = address;

            Type type = parameters.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                string name = property.Name;
                object value = property.GetValue(parameters);

                Parameters.Add(name.ToLower(), value);
            }
        }

        public override string ToString()
        {
            return $"{Method}://{Address}";
        }
    }

    /// <summary>
    /// Describes a link between two Bedrock devices
    /// </summary>
    public class LinkInfo : PathNodeInfo
    {
        public DeviceInfo Source { get; }

        public LinkInfo(DeviceInfo source, DeviceInfo destination)
        {
            Source = source;
            Destination = destination;
        }

        public override string ToString()
        {
            return $"{Source.Name} -> {Destination.Name}";
        }
    }
}