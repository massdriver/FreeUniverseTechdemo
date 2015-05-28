using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.ViewControllers
{
    public enum GUIWindowTypes
    {
        Null,
        PopupMessageBox,
        CreateAccount,

        Unknown
    }

    public class GUIStaticData
    {
        public static int WND_CLIENTLOGINVIEW_LOGIN = 100;
        public static int WND_CLIENTLOGINVIEW_WAIT = 101;
        public static int WND_CLIENTLOGINVIEW_REFUSED = 102;
        public static int WND_CHARACTERSELECTION_TABLE = 103;
        public static int WND_TEST_LOGIN_VIEW = 104;

        public const float WindowHorizontalOffset = 10.0f;
        public const float WindowVerticalOffset = 20.0f;
    }
}
