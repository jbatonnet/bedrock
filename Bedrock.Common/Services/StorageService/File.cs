using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bedrock.Common
{
    public abstract class File : Entry
    {
        public abstract DateTime Date { get; set; }
        public abstract ulong Size { get; }
        public abstract uint Hash { get; }
        
        public abstract Stream Open(FileAccess access);

        public override string ToString()
        {
            return Path;
        }
    }
}