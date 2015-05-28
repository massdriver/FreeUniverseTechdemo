using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Net.Messages
{
    public class MsgTZSConnectReply : NetworkMessage
    {
        public int client { get; set; }

        public MsgTZSConnectReply()
        {
            type = NetworkMessageType.TZSConnectReply;
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);

            client = msgIn.ReadInt32();

            return this;
        }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);

            msgOut.Write(client);

            return this;
        }
    }
}
