using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bedrock.Common
{
    public enum ConnectionSpeed
    {
        /// <summary>
        /// Unknown speed
        /// </summary>
        Unknown,

        /// <summary>
        /// Slow connection (&lt; 50 kB/s)
        /// </summary>
        Slow,

        /// <summary>
        /// Normal connection (&lt; 500 kB/s)
        /// </summary>
        Normal,

        /// <summary>
        /// Fast connection (&gt; 500 kB/s)
        /// </summary>
        Fast
    }

    public enum ConnectionLatency
    {
        /// <summary>
        /// Unknown latency
        /// </summary>
        Unknown,

        /// <summary>
        /// Low latency (&lt; 100 ms)
        /// </summary>
        Low,

        /// <summary>
        /// Normal latency (&lt; 1 s)
        /// </summary>
        Normal,

        /// <summary>
        /// High latency (&gt; 1 s)
        /// </summary>
        High
    }

    public enum ConnectionReliability
    {
        /// <summary>
        /// Unknown reliability
        /// </summary>
        Unknown,

        /// <summary>
        /// Low reliability
        /// </summary>
        Low,

        /// <summary>
        /// High reliability
        /// </summary>
        High
    }

    /// <summary>
    /// Represents a connection to a Bedrock server
    /// </summary>
    public abstract class Connection : IDisposable
    {
        public abstract ConnectionSpeed Speed { get; }
        public abstract ConnectionLatency Latency { get; }
        public abstract ConnectionReliability Reliability { get; }

        /// <summary>
        /// Retrieve the remote server object from this connection
        /// </summary>
        /// <returns>The remote server object</returns>
        public abstract Task<DeviceHub> GetServer();

        public virtual void Dispose() { }
    }
}