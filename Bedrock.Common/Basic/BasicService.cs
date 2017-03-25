using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bedrock.Common;

namespace Bedrock.Shared
{
    public abstract class BasicService : Service
    {
        public virtual string[] Actions { get; } = new string[0];

        public abstract void Execute(string action);
    }
}