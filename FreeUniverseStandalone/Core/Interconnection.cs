using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Interconnection
{
    public interface IServer
    {
        void OnInit();
        void OnUpdate(float dt);
        void OnQuit();
    }

    public interface IServerPlugin
    {

    }
}
