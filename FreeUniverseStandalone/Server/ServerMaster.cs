using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.World.Database;
using UnityEngine;
using FreeUniverse.Core;
using FreeUniverse.Common.Database;

namespace FreeUniverse.Server
{
    public class ServerMaster
    {
        public WorldDatabaseMaster wbd { get; set; }
        private ServerLogin loginServer { get; set; }
        private ServerChat chatServer { get; set; }
        
        public ServerMaster()
        {
            wbd = new WorldDatabaseMaster();
            loginServer = new ServerLogin();
            chatServer = new ServerChat();

            loginServer.wdb = wbd;
        }

        public void Start()
        {
            loginServer.Start();

            Debug.Log("MasterServer: started");
        }

        public void Shutdown()
        {
            loginServer.Shutdown();
            wbd.Shutdown();

            Debug.Log("MasterServer: stopped");
        }

        public void Update()
        {
            loginServer.Update();
        }

   
    }
}
