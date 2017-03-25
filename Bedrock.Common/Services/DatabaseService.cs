using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bedrock.Common
{
    public class DatabaseService : Service
    {
        public DbConnection Connection { get; set; }
    }
}