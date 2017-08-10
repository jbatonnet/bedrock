using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Bedrock.Common
{
    /// <summary>
    /// Describes a Bedrock server
    /// </summary>
    public class ServerInfo : DeviceInfo
    {
        public IEnumerable<ConnectionInfo> Connections => connections.AsReadOnly();

        private List<ConnectionInfo> connections = new List<ConnectionInfo>();

        public ServerInfo(DeviceInfo deviceInfo, IEnumerable<ConnectionInfo> connectionsInfo) : base(deviceInfo.Id, deviceInfo.Name, deviceInfo.Description, deviceInfo.Hub)
        {
            connections = connectionsInfo.ToList();
        }
        public ServerInfo(Guid id, string name, string description, bool hub) : base(id, name, description, hub)
        {
        }

        /// <summary>
        /// Establish a connection to the server
        /// </summary>
        /// <returns>The server connection</returns>
        public Task<Connection> Connect()
        {
            return Connect(ConnectionSpeed.Unknown, ConnectionLatency.Unknown);
        }
        /// <summary>
        /// Establish a connection matching the specified speed criteria to the server
        /// </summary>
        /// <param name="speed">The speed criteria to select the connection</param>
        /// <returns>The server connection</returns>
        public Task<Connection> Connect(ConnectionSpeed speed)
        {
            return Connect(speed, ConnectionLatency.Unknown);
        }
        /// <summary>
        /// Establish a connection matching the specified latency criteria to the server
        /// </summary>
        /// <param name="latency">The latency criteria to select the connection</param>
        /// <returns>The server connection</returns>
        public Task<Connection> Connect(ConnectionLatency latency)
        {
            return Connect(ConnectionSpeed.Unknown, latency);
        }
        /// <summary>
        /// Establish a connection matching the specified speed and latency criterias to the server
        /// </summary>
        /// <param name="speed">The speed criteria to select the connection</param>
        /// <param name="latency">The latency criteria to select the connection</param>
        /// <returns>The server connection</returns>
        public async Task<Connection> Connect(ConnectionSpeed speed, ConnectionLatency latency)
        {
            return await Task.Run(() =>
            {
                ConnectionInfo bestConnection = connections.FirstOrDefault();
                if (bestConnection == null)
                    return null;

                IEnumerable<Type> connectionTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.IsSubclassOf(typeof(Connection)));

                Type connectionType = connectionTypes.Single(t => t.GetCustomAttribute<LinkMethodAttribute>()?.Method == bestConnection.Method);

                ConstructorInfo[] connectionTypeConstructors = connectionType.GetConstructors();
                foreach (ConstructorInfo constructor in connectionTypeConstructors)
                {
                    ParameterInfo[] parameters = constructor.GetParameters();
                    object[] parameterValues = new object[parameters.Length];

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        string name = parameters[i].Name.ToLower();

                        if (name == "address")
                            parameterValues[i] = bestConnection.Address;
                        else
                        {
                            object value;
                            if (bestConnection.Parameters.TryGetValue(name, out value))
                                parameterValues[i] = value;
                        }
                    }

                    try
                    {
                        // Try to convert parameters
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            Type parameterType = parameters[i].ParameterType;
                            if (parameterValues[i] == null && !parameterType.IsClass)
                                continue;

                            Type valueType = parameterValues[i].GetType();
                            if (parameterType.IsAssignableFrom(valueType))
                                continue;

                            parameterValues[i] = Convert.ChangeType(parameterValues[i], parameterType);
                        }

                        // Invoke the constructor
                        return (Connection)constructor.Invoke(parameterValues);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }

                throw new Exception("Could not build a matching connection");
            });
        }
    }
}