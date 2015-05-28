using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;

namespace FreeUniverse.Common
{
    public class CargoItem
    {
        public ulong commodity;
        public uint quantity;
        public ArchObjectCommodity commodityArchRef;
    }

    public class CargoHoldData
    {
        public float maxCapacity { get; private set; }
        public float usedCapacity { get; private set; }
        public Dictionary<ulong, CargoItem> cargoItems { get; private set; }
    }
}
