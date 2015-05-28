using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Common;
using FreeUniverse.Core.Serialization;

namespace FreeUniverse.Net.Messages
{
    public class MsgLSAccountCharacterAndSolarDataReply : NetworkMessage
    {
        private static FastSerializer serializer = new FastSerializer(typeof(CharacterPersonality));

        public List<CharacterPersonality> characters { get; set; }

        public MsgLSAccountCharacterAndSolarDataReply()
        {
            type = NetworkMessageType.LSAccountCharacterAndSolarDataReply;
        }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);

            msgOut.Write(characters.Count);

            foreach (CharacterPersonality e in characters)
                WriteBytes(serializer.SerializeToBytes(e), msgOut);

            return this;
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);

            int chars = msgIn.ReadInt32();

            for (int i = 0; i < chars; i++)
                characters.Add( serializer.DeserializeFromBytes( ReadBytes(msgIn)) as CharacterPersonality);

            return this;
        }
    }
}
