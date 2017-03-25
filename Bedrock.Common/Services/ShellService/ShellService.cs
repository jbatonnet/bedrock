using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bedrock.Common
{
    public abstract class ShellService : Service
    {
        public abstract ShellSession OpenSession();
    }
}