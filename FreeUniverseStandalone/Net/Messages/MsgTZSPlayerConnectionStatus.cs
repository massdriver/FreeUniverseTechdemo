using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Net.Messages
{
    public class MsgTZSPlayerConnectionStatus : NetworkMessage
    {
        public string playerName { get; set; }
        public int status { get; set; }

        public const int STATUS_CONNECTED = 1;
        public const int STATUS_DISCONNECTED = 2;

        public MsgTZSPlayerConnectionStatus()
        {
            type = NetworkMessageType.TZSPlayerConnectionStatus;
            status = -1;
            playerName = "";
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);

            status = msgIn.ReadInt32();
            playerName = msgIn.ReadString();

            return this;
        }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);

            msgOut.Write(status);
            msgOut.Write(playerName);

            return this;
        }
    }
}
