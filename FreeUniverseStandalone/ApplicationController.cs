using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeUniverse.World;
using FreeUniverse.Core;
using FreeUniverse.ViewControllers;
using FreeUniverse.Client;
using FreeUniverse.GUILib;
using System.Threading;
using FreeUniverse.Arch;

namespace FreeUniverse
{
    public class ApplicationController : INotificationCenterDelegate
    {
        public ApplicationController()
        {

        }

        public void OnInit()
        {
            NotificationCenter.AddDelegate(this, NotificationID.ConsoleInput);

            GUIExtensions.Init();
            AudioManager.Init();
            ResourceManager.Init();
            ArchModel.Init();

            ApplicationModel.viewControllerManager = new ViewControllerManager();
            ApplicationModel.consoleViewController = new ViewControllerConsole();
            ApplicationModel.gameClient = new GameClient();

            ConsoleLog.Write("Free Universe " + ConstData.VERSION);

            ApplicationModel.masterServer.Start();
            //ApplicationModel.testZoneServer.Start();
            ApplicationModel.gameClient.Start();
        }
        

        public void OnGUI()
        {
            if (ApplicationModel.gameClient != null)
                ApplicationModel.gameClient.OnGUI();

            if (ApplicationModel.viewControllerManager != null)
                ApplicationModel.viewControllerManager.OnGUI();
        }

        public void OnUpdate()
        {
            NotificationCenter.Update();

            float timeDelta = Time.deltaTime;

            ApplicationModel.inputController.Update(timeDelta);

            if( ApplicationModel.masterServer != null )
                ApplicationModel.masterServer.Update();

            if (ApplicationModel.gameClient != null)
                ApplicationModel.gameClient.Update(timeDelta);

            if (ApplicationModel.testZoneServer != null)
                ApplicationModel.testZoneServer.Update(timeDelta);

            ResourceManager.Update(timeDelta);
        }

        public void OnQuit()
        {
            if (ApplicationModel.gameClient != null)
                ApplicationModel.gameClient.Shutdown();
        }

        #region INotificationCenterDelegate Members

        public void OnNotificationCenterEvent(NotificationID id, object data)
        {
            if (id == NotificationID.ConsoleInput)
            {
                string cmd = data as string;

                if (cmd.CompareTo("/zs") == 0)
                {
                    ApplicationModel.testZoneServer.Start();
                }

            }
        }

        #endregion
    }
}
