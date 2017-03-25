using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bedrock.Common
{
    public enum ClipboardDataType
    {
        Text,
        Image,
    }

    public struct ClipboardData
    {
        public ClipboardDataType Type { get; private set; }
        public object Value { get; private set; }

        public ClipboardData(ClipboardDataType type, object value)
        {
            Type = type;
            Value = value;
        }
    }

    public abstract class ClipboardService : Service
    {
        public abstract ClipboardData Data { get; set; }

        public event EventHandler<ClipboardData> Copy;

        protected void OnCopy(ClipboardData data)
        {
            Copy?.Invoke(this, data);
        }
    }
}