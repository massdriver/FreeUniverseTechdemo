using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Net.Messages
{
    public class MsgTZSDestroyWorldObjectComponent : NetworkMessage
    {
        public MsgTZSDestroyWorldObjectComponent()
        {
            type = NetworkMessageType.TZSDestroyWorldObjectComponent;
        }

        public int obj { get; set; }
        public int component { get; set; }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);

            msgOut.Write(obj);
            msgOut.Write(component);

            return this;
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);

            obj = msgIn.ReadInt32();
            component = msgIn.ReadInt32();

            return this;
        }
    }
}
