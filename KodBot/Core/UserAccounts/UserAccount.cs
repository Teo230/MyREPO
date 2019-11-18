using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodBot.Core.UserAccounts
{
    class UserAccount
    {
        public ulong ID { get; set; }
        public string Nick { get; set; }
        public uint XP { get; set; }
        public uint Lvl 
        {
            get
            {
                return (uint)Math.Sqrt(XP / 10);
            }
            set { }
        }
        public string P { get; set; }
        public string S { get; set; }
        public string C { get; set; }
    }
}