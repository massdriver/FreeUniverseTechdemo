using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Common;

namespace FreeUniverse.Arch
{
    public class ArchShipPackage : ArchTradeable
    {

    }

    public class ArchCommodityPackage : ArchTradeable
    {

    }

    public class ArchMissionPackage : ArchObject
    {

    }

    public class ArchObjectBaseContents : ArchObject
    {
        public CreditType localCurrency { get; set; }
        public List<ArchShipPackage> shipPackages { get; set; }
        public List<ArchCommodityPackage> commodityPackages { get; set; }
        public List<ArchMissionPackage> missionsPackages { get; set; }

        public ArchObjectBaseContents()
        {
            localCurrency = CreditType.UniverseCredit;
            shipPackages = new List<ArchShipPackage>();
            commodityPackages = new List<ArchCommodityPackage>();
            missionsPackages = new List<ArchMissionPackage>();
        }
    }
}
