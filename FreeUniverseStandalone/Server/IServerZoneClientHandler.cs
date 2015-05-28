using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Net;

namespace FreeUniverse.Server
{
    public interface IServerZoneClientHandler
    {
        void OnConnect(int client);
        void OnDisconnect(int client);
        void OnMessage(int client, NetworkMessage msg);
        void OnUpdate(float time);
    }
}
