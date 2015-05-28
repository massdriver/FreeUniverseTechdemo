using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Net.Messages
{
    public class MsgCLAccountLoginRequest : NetworkMessage
    {
        public string email { get; set; }
        public string password { get; set; }

        public MsgCLAccountLoginRequest()
        {
            type = NetworkMessageType.CLAccountLoginRequest;
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);

            email = msgIn.ReadString();
            password = msgIn.ReadString();

            return this;
        }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);

            msgOut.Write(email);
            msgOut.Write(password);

            return this;
        }
    }
}
