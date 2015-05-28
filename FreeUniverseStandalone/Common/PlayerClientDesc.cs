using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.World;

namespace FreeUniverse.Common
{
    public enum PlayerClientStatus
    {
        Unknown,
        LoginServer,
        Transit,
        ZoneServer,
        AwaitLogout
    }

    public enum PlayerClientDataLocation
    {
        Unknown,
        LoginServer,
        ZoneServer
    }

    public class ClientDesc
    {
        public PlayerClientDataLocation dataLocation { get; set; }
        public PlayerClientStatus status { get; set; }
        public int client { get; set; }
        public uint guid { get; set; }

        public Account account { get; set; }

        // Test zone server data
        public ulong system { get; set; }
        public int worldObject { get; set; }
        public string playerNickname { get; set; }
        public ulong character { get; set; }
        public ulong template { get; set; }
        public WorldControllerServer world { get; set; }

        public ClientDesc()
        {
            Reset();
        }

        // Only for test case
        public bool inWorld
        {
            get
            {
                return world != null && client != -1;
            }
        }

        public void Reset()
        {
            dataLocation = PlayerClientDataLocation.Unknown;
            account = null;
            status = PlayerClientStatus.Unknown;
            guid = 0;
            template = 0;
            world = null;
            client = -1;
            system = 0;
            worldObject = -1;
            playerNickname = "";
            character = 0;
        }
    }
}
