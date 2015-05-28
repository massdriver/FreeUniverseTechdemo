using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Net.Messages
{
    public class MsgTZSSetWorldObjectName : NetworkMessage
    {
        public MsgTZSSetWorldObjectName()
        {
            type = NetworkMessageType.TZSSetWorldObjectName;
            name = "";
        }

        public int obj { get; set; }
        public string name { get; set; }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);

            msgOut.Write(obj);
            msgOut.Write(name);

            return this;
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);

            obj = msgIn.ReadInt32();
            name = msgIn.ReadString();

            return this;
        }
    }
}
