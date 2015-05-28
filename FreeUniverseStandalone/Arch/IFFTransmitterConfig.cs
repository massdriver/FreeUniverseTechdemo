using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Arch
{
    public enum IFFSignatureType
    {
        Friendly,
        Neutral,
        Hostile
    }

    public class IFFTransmitterConfig
    {
        Dictionary<ulong, IFFSignatureType> _signatures = new Dictionary<ulong, IFFSignatureType>();

        public IFFSignatureType Get(ulong faction)
        {
            IFFSignatureType signature = IFFSignatureType.Neutral;
            _signatures.TryGetValue(faction, out signature);
            return signature;
        }

        public void Set(ulong faction, IFFSignatureType type)
        {
            _signatures[faction] = type;
        }
    }
}
