using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Net.Messages
{
    public class MsgTZSComponentHullData : NetworkMessage
    {
        public MsgTZSComponentHullData()
        {
            type = NetworkMessageType.TZSComponentHullData;
        }

        public int obj { get; set; }
        public int component { get; set; }
        public float structure { get; set; }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);

            msgOut.Write(obj);
            msgOut.Write(component);
            msgOut.Write(structure);

            return this;
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);

            obj = msgIn.ReadInt32();
            component = msgIn.ReadInt32();
            structure = msgIn.ReadFloat();

            return this;
        }
    }
}
