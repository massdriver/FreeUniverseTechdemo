using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Net.Messages
{
    public class MsgCLAccountCharacterAndSolarDataRequest : NetworkMessage
    {
        public MsgCLAccountCharacterAndSolarDataRequest()
        {
            type = NetworkMessageType.CLAccountCharacterAndSolarDataRequest;
        }
    }
}
