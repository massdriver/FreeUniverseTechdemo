using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Core
{
    public enum NotificationID
    {
        Null = 0,

        LoginServerAuthorization,  // object is int value that contains result
        ConsoleInput, // user submits console text, cast data object to string
        LoginServerConnectionLost,

        Unknown
    }
}
