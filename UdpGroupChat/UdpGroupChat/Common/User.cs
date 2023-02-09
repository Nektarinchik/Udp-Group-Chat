using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdpGroupChat.Common
{
    internal class User
    {
        public string? Name { get; set; }
        public User() : this(null)
        { }
        public User(string? name)
        {
            Name = name;
        }
    }
}
