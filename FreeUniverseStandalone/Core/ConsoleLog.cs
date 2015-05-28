using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.ViewControllers;

namespace FreeUniverse.Core
{
    public class ConsoleLog
    {
        public static void Write(string str)
        {
            if(ViewControllerConsole.instance != null)
                ViewControllerConsole.instance.Log(str, false);
        }
    }
}
