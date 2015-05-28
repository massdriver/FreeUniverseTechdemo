using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Core.Serialization;

namespace FreeUniverse.Common
{
    [FastSerializable]
    public class PremiumStorageItem
    {
        [FastSerializable]
        public ulong product { get; set; }

        [FastSerializable]
        public int count { get; set; }
    }

    public class PremiumStorage
    {
        [FastSerializable]
        public Dictionary<ulong, PremiumStorageItem> items { get; set; }
    }
}
