using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Core
{
    public struct uidkey
    {
        public ulong id0 { get; set; }
        public ulong id1 { get; set; }

        public uidkey(string str) : this()
        {
            if (str == null)
            {
                id0 = 0;
                id1 = 0;
                return;
            }

            Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            UInt32 h1 = HashUtils.Hash(bytes);
            UInt32 h2 = HashUtils.Hash(bytes, h1);
            id0 = (((ulong)h1) << 32) + (ulong)h2;

            h1 = HashUtils.Hash(bytes, h2);
            h2 = HashUtils.Hash(bytes, h1);

            id1 = (((ulong)h1) << 32) + (ulong)h2;
        }

        public uidkey(ulong i0, ulong i1)
            : this()
        {
            id0 = i0;
            id1 = i1;
        }

        public override string ToString()
        {
            return "uidkey=" + id0.ToString() + id1.ToString();
        }

        public static bool operator ==(uidkey a, uidkey b)
        {
            return a.id0 == b.id0 && a.id1 == b.id1;
        }

        public static bool operator !=(uidkey a, uidkey b)
        {
            return a.id0 != b.id0 || a.id1 != b.id1;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj.GetType().Equals(typeof(uidkey)) && (((uidkey)obj)==this);
        }
    }
}
