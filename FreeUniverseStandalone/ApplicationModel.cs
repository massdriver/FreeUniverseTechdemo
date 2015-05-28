using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;
using FreeUniverse.World;
using FreeUniverse.ViewControllers;
using FreeUniverse.Server;
using FreeUniverse.Client;
using FreeUniverse.Core;

namespace FreeUniverse
{
    public class ApplicationModel
    {
        public static NotificationCenter notoficationCenter = new NotificationCenter();

        public static StringModel stringModel = new StringModel();

        public static ServerMaster masterServer = new ServerMaster();
        public static InputController inputController = new InputController();
        public static GameClient gameClient;
        public static ViewControllerConsole consoleViewController;
        public static ViewControllerManager viewControllerManager;

        public static ServerZone testZoneServer = new ServerZone();
        
    }
}
