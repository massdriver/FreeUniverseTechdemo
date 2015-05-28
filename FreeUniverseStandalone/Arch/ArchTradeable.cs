using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Arch
{
    public class ArchTradeable : ArchObject
    {
        public ulong universalPrice { get; set; }
        public float itemVolume { get; set; }

        public float sellPriceModificator { get; set; }
        public float buyPriceModificator { get; set; }

        public ArchTradeable()
        {
            sellPriceModificator = 1.0f;
            buyPriceModificator = 1.0f;
        }

        public ulong buyPrice
        {
            get
            {
                return (ulong)(universalPrice * buyPriceModificator);
            }
        }

        public ulong sellPrice
        {
            get
            {
                return (ulong)(universalPrice * sellPriceModificator);
            }
        }

        public override void ReadParameter(INIReaderParameter parameter)
        {
            base.ReadParameter(parameter);

            if (parameter.Check("universal_price"))
            {
                universalPrice = parameter.GetUInt64(0);
            }
            else if(parameter.Check("item_volume"))
            {
                itemVolume = parameter.GetFloat(0);
            }
        }
    }
}
