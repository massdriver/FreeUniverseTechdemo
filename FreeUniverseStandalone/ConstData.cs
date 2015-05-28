using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse
{
    public class ConstData
    {
        public static int CLIENT_PORT = 14256;
        public static int LAYER_STATIC_BACKGROUND = 8;
        public static int LAYER_CLIENT_WORLD = 10;
        public static int LAYER_SERVER_WORLD = 9;

        public static int DEFAULT_MAX_CLIENTS = 32;
        public static string VERSION = "v0.1";

        public static int CONSOLE_GUI_DEPTH = 1;
        public static int DEFAULT_MENU_GUI_DEPTH = 2;
        public static int HUD_GUI_DEPTH = 3;

        public static int MAX_WORLD_OBJECTS = 4096;
        public const int MAX_CLIENTS = 64;

        public const int LOGIN_SERVER_PORT = 26600;

        public static ulong TEST_SYSTEM = HashUtils.StringToUINT64("system_demo");

        public const float RANDOM_POSITION_FACTOR = 300.0f;

        public const string DATABASE_NAME = "free_universe_database";
        public const string DATABASE_USER = "freeuniverse";
        public const string DATABASE_PASSWORD = "freeuniverse";

        public const bool USE_INDEX_ARCH = false;
    }
}
