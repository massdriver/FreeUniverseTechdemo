using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Net.Messages
{
    public class MsgLSCreateAccountReply : NetworkMessage
    {
        byte _reply;

        public static byte ACCOUNT_CREATED = 1;
        public static byte ACCOUNT_EXISTS = 2;
        public static byte ACCOUNT_NOT_CREATED = 3;

        public MsgLSCreateAccountReply()
        {
            type = NetworkMessageType.LSCreateAccountReply;
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);
            msgIn.ReadByte(out _reply);
            return this;
        }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);
            msgOut.Write(_reply);
            return this;
        }
    }
}
