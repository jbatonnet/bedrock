using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Bedrock.Common
{
    public class User
    {
        public static User Guest { get; } = new User(new Guid("eb31a796-4ef4-4f46-b787-17308532c098"), nameof(Guest));

        public Guid Id { get; }

        public string Name { get; }
        public string FullName { get; }

        private User(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}