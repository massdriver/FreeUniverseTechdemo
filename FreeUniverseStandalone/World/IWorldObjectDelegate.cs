using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.World
{
    public interface IWorldObjectDelegate
    {
        void OnWorldObjectReleased(WorldObject obj);
    }
}
