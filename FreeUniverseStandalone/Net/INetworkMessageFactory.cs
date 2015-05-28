using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace FreeUniverse.Net
{
    public interface INetworkMessageFactory
    {
        NetworkMessage Create(NetIncomingMessage msg);
    }
}
