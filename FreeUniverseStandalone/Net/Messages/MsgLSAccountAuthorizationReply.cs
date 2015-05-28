using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.World.Database;
using FreeUniverse.Core;
using FreeUniverse.Common;
using FreeUniverse.Core.Serialization;

namespace FreeUniverse.Net.Messages
{
    public class MsgLSAccountAuthorizationReply : NetworkMessage
    {
        private static FastSerializer serializer = new FastSerializer(typeof(Account));
        public Account account { get; set; }
        public int reply { get; set; }

        public static int AUTHORIZATION_FAILED = -1;

        public MsgLSAccountAuthorizationReply()
        {
            type = NetworkMessageType.LSAccountAuthorizationReply;
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);
            reply = msgIn.ReadInt32();

            if (reply > 0)
            {
                int len = msgIn.ReadInt32();
                byte[] data = msgIn.ReadBytes(len);
                account = serializer.DeserializeFromBytes(data) as Account;
            }

            return this;
        }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);

            msgOut.Write(reply);

            if (reply > 0)
            {
                byte[] data = serializer.SerializeToBytes(account);
                msgOut.Write(data.Length);
                msgOut.Write(data);
            }

            return this;
        }
    }
}
