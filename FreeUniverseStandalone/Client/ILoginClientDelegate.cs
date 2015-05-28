using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.World.Database;
using FreeUniverse.Common;

namespace FreeUniverse.Client
{
    public interface ILoginClientDelegate
    {
        void OnLoginClientConnectionAccept(LoginClient client, Account account);
        void OnLoginClientConnectionLost(LoginClient client);
        void OnLoginClientConnectionRefused(LoginClient client);
    }
}
