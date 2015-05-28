using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.World
{
    public class WorldObjectControllerAIDummy : WorldObjectControllerAI
    {
        public WorldObjectControllerAIDummy(WorldObject obj)
        {
            SetWorldObject(obj);
        }

        public override void Update(float time)
        {
            if ( controlledObject == null) return;

            WorldObjectControlPanel panel = controlledObject.controlPanel;

            panel.SetEngineControlValues(1.0f, 0.0f, 0.0f, 0.2f, 0.0f, 0.0f);
        }
    }
}
