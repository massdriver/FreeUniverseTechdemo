using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Net.Messages
{
    public class MsgTFireAllActiveWeapons : NetworkMessage
    {
        public MsgTFireAllActiveWeapons()
        {
            type = NetworkMessageType.TFireAllActiveWeapons;
        }

        public int obj { get; set; }
        public uint weapons { get; set; }
        public Vector3 targetPoint { get; set; }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);

            msgOut.Write(obj);
            msgOut.Write(weapons);
            this.WriteVector3(targetPoint, msgOut);

            return this;
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);

            obj = msgIn.ReadInt32();
            weapons = msgIn.ReadUInt32();
            targetPoint = this.ReadVector3(msgIn);

            return this;
        }
    }
}
