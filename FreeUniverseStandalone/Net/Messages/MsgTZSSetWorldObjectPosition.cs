using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Net.Messages
{
    public class MsgTZSSetWorldObjectPosition : NetworkMessage
    {
        public MsgTZSSetWorldObjectPosition()
        {
            type = NetworkMessageType.TZSSetWorldObjectPosition;
        }

        public int obj { get; set; }
        public int component { get; set; }
        public Vector3 position { get; set; }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);

            msgOut.Write(obj);
            msgOut.Write(component);
            this.WriteVector3(position, msgOut);

            return this;
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);

            obj = msgIn.ReadInt32();
            component = msgIn.ReadInt32();
            position = this.ReadVector3(msgIn);

            return this;
        }
    }
}
