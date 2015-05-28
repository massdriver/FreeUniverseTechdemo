using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Core.Serialization;

namespace FreeUniverse.Common.Database
{
    [FastSerializable]
    public class DatabaseElement<T>
    {
        [FastSerializable]
        public ulong id { get; set; }

        [FastSerializable]
        public ulong parent { get; set; }

        [FastSerializable]
        public uint mark { get; set; }
    }
}
