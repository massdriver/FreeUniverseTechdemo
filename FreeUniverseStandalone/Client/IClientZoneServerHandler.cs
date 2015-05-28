using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Net;

namespace FreeUniverse.Client
{
    public interface IClientZoneServerHandler
    {
        void OnConnect();
        void OnDisconnect();
        void OnMessage(NetworkMessage msg);
        void OnUpdate(float time);
    }
}
