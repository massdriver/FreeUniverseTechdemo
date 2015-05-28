using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Net
{
    public enum NetworkMessageType
    {
        Null,

        // Client & Login server protocol
        CLAccountLoginRequest,
        LSAccountAuthorizationReply,

        CLCreateCharacterRequest,
        LSCreateCharaterReply,
        CLAccountCharacterAndSolarDataRequest,
        LSAccountCharacterAndSolarDataReply,

        CLCreateAccountRequest,
        LSCreateAccountReply,

        // Debug purpose protocol between test zone server and test zone client versions
        TestProtocol = 1024,

        TZCConnect,
        TZSConnectReply,
        TZSPlayerConnectionStatus,
        TZSCreateWorldObject,
        TZSControlWorldObjectByClient,
        TZCWorldObjectBasicUpdate,
        TFireAllActiveWeapons,
        TZCClientProjectileHit,
        TZSComponentHullData,
        TZSDestroyWorldObjectComponent,
        TZSRemoveWorldObject,
        TZSSetWorldObjectName,
        TZSSetWorldObjectPosition,

        Unknown
    }
}
