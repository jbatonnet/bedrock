using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bedrock.Common
{
    /// <summary>
    /// A simple Bedrock service to push data to a device
    /// </summary>
    public abstract class PushService : Service
    {
        /// <summary>
        /// Pushes the specified text to this device
        /// </summary>
        /// <param name="text">The text to send</param>
        public abstract void PushText(string text);

        /// <summary>
        /// Pushes the specified uri to this device
        /// </summary>
        /// <param name="uri">The uri to send</param>
        public abstract void PushUri(Uri uri);

        /// <summary>
        /// Pushes the specified buffer to this device
        /// </summary>
        /// <param name="buffer">The buffer to send</param>
        public abstract void PushImage(byte[] buffer);
    }
}