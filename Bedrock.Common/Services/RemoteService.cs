using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bedrock.Common
{
    public enum RemoteKey
    {
        Number0,
        Number1,
        Number2,
        Number3,
        Number4,
        Number5,
        Number6,
        Number7,
        Number8,
        Number9,

        Left, Up, Right, Down,
        Ok, Back,

        VolumeUp,
        VolumeDown,
        VolumeMute,
    }

    public abstract class RemoteService : Service
    {
        public virtual RemoteKey[] Keys { get; }

        public abstract void Press(RemoteKey key);
    }
}