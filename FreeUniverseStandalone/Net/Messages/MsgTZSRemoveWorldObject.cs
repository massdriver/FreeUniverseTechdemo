using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Net.Messages
{
    public class MsgTZSRemoveWorldObject : NetworkMessage
    {
        public MsgTZSRemoveWorldObject()
        {
            type = NetworkMessageType.TZSRemoveWorldObject;
        }

        public int obj { get; set; }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);

            msgOut.Write(obj);

            return this;
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);

            obj = msgIn.ReadInt32();

            return this;
        }
    }
}
