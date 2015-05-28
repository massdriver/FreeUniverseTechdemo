using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Net.Messages
{
    public class MsgTZCWorldObjectBasicUpdate : NetworkMessage
    {
        public MsgTZCWorldObjectBasicUpdate()
        {
            type = NetworkMessageType.TZCWorldObjectBasicUpdate;
        }

        public int obj { get; set; }
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);

            msgOut.Write(obj);
            this.WriteVector3(position, msgOut);
            this.WriteQuaternion(rotation, msgOut);

            return this;
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);

            obj = msgIn.ReadInt32();
            position = this.ReadVector3(msgIn);
            rotation = this.ReadQuaternion(msgIn);

            return this;
        }
    }
}
