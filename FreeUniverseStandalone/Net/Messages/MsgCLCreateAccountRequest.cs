using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Net.Messages
{
    public class MsgCLCreateAccountRequest : NetworkMessage
    {
        string _user = "";
        string _password = "";

        public string user { get; set; }
        public string password { get; set; }

        public MsgCLCreateAccountRequest()
        {
            type = NetworkMessageType.CLCreateAccountRequest;
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);

            msgIn.ReadString(out _user);
            msgIn.ReadString(out _password);

            return this;
        }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);

            msgOut.Write(_user);
            msgOut.Write(_password);

            return this;
        }
    }
}
