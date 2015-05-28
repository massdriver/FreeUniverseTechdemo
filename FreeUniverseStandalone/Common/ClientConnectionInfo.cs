using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Core;

namespace FreeUniverse.Common
{
    public struct IPAddress
    {
        public string ip;
        public int port;
    }

    public class ClientConnectionInfo
    {
        public IPAddress tradeServerAddress { get; set; }
        public IPAddress chatServerAddress { get; set; }
        public IPAddress zoneServerAddress { get; set; }
        public IPAddress neuralNetServerAddress { get; set; }
        public int clientid { get; set; }
        public uidkey clientSessionGUID { get; set; }
    }
}
