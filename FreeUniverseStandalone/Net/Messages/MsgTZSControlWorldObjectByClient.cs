using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Net.Messages
{
    public class MsgTZSControlWorldObjectByClient : NetworkMessage
    {
        public int obj { get; set; }
        public int client { get; set; }
        public int status { get; set; }

        public const int STATUS_ENABLED = 1;
        public const int STATUS_DISABLED = 2;

        public MsgTZSControlWorldObjectByClient()
        {
            type = NetworkMessageType.TZSControlWorldObjectByClient;
        }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);

            msgOut.Write(obj);
            msgOut.Write(client);
            msgOut.Write(status);

            return this;
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);

            obj = msgIn.ReadInt32();
            client = msgIn.ReadInt32();
            status = msgIn.ReadInt32();

            return this;
        }
    }
}
