using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Bedrock.Common
{
    public class PlatformShellService : ShellService
    {
        public override ShellSession OpenSession()
        {
            return new PlatformShellSession();
        }
    }
}
