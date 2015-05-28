using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Net.Messages
{
    public class MsgTZSCreateWorldObject : NetworkMessage
    {
        public int id { get; set; }
        public ulong component { get; set; }
        public ulong guid { get; set; }
        public string nickname { get; set; }
        public Vector3 position { get; set; } // not implemented

        public MsgTZSCreateWorldObject()
        {
            type = NetworkMessageType.TZSCreateWorldObject;
            nickname = "";
        }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);

            msgOut.Write(id);
            msgOut.Write(component);
            msgOut.Write(guid);
            msgOut.Write(nickname);

            return this;
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);

            id = msgIn.ReadInt32();
            component = msgIn.ReadUInt64();
            guid = msgIn.ReadUInt64();
            nickname = msgIn.ReadString();

            return this;
        }
    }
}
