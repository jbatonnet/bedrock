using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bedrock.Common
{
    public abstract class Directory : Entry
    {
        public abstract IEnumerable<Directory> Directories { get; }
        public abstract IEnumerable<File> Files { get; }
        
        public virtual Directory GetDirectory(string path)
        {
            int separator = path.IndexOf('/');

            if (separator >= 0)
                return Directories.SingleOrDefault(d => d.Name == path.Substring(0, separator))?.GetDirectory(path.Substring(separator + 1));
            else
                return Directories.SingleOrDefault(d => d.Name == path);
        }
        public virtual File GetFile(string path)
        {
            int separator = path.IndexOf('/');

            if (separator >= 0)
                return Directories.SingleOrDefault(d => d.Name == path.Substring(0, separator))?.GetFile(path.Substring(separator + 1));
            else
                return Files.SingleOrDefault(d => d.Name == path);
        }

        public abstract Directory CreateDirectory(string name);
        public abstract void DeleteDirectory(Directory directory);

        public abstract File CreateFile(string name);
        public abstract void DeleteFile(File file);

        public override string ToString()
        {
            return Path;
        }
    }
}