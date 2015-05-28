using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Net.Messages
{
    public class MsgTZCConnect : NetworkMessage
    {
        string _playerNickname;

        public string playerNickname
        {
            get { return _playerNickname; }
            set { _playerNickname = value; }
        }
        ulong _startTemplate;

        public ulong startTemplate
        {
            get { return _startTemplate; }
            set { _startTemplate = value; }
        }

        public MsgTZCConnect()
        {
            type = NetworkMessageType.TZCConnect;
            _playerNickname = "";
        }

        public override NetworkMessage Read(Lidgren.Network.NetIncomingMessage msgIn)
        {
            base.Read(msgIn);

            _playerNickname = msgIn.ReadString();
            _startTemplate = msgIn.ReadUInt64();

            return this;
        }

        public override NetworkMessage Write(Lidgren.Network.NetOutgoingMessage msgOut)
        {
            base.Write(msgOut);

            msgOut.Write(_playerNickname);
            msgOut.Write(_startTemplate);

            return this;
        }
    }
}
