using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;

namespace FreeUniverse.Net.Messages
{
    public class MsgTZCClientProjectileHit : NetworkMessage
    {
        public MsgTZCClientProjectileHit()
        {
            type = NetworkMessageType.TZCClientProjectileHit;
            archIndex = ArchObject.INVALID_INDEX;
        }

        public int obj { get; set; }
        public int component { get; set; }
        public ulong arch { get; set; }
        public int archIndex { get; set; }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);

            msgOut.Write(obj);
            msgOut.Write(component);
            msgOut.Write(arch);
            msgOut.Write(archIndex);

            return this;
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);

            obj = msgIn.ReadInt32();
            component = msgIn.ReadInt32();
            arch = msgIn.ReadUInt64();
            archIndex = msgIn.ReadInt32();

            return this;
        }
    }
}
