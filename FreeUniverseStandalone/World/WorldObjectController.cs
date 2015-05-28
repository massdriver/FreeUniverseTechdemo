using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.World
{
    public abstract class WorldObjectController : IWorldObjectDelegate
    {
        public WorldObject controlledObject { get; protected set; }

        public virtual void SetWorldObject(WorldObject obj)
        {
            if (controlledObject != null)
                controlledObject.RemoveDelegate(this);

            controlledObject = obj;

            if (controlledObject != null)
                controlledObject.AddDelegate(this);
        }

        public abstract void Update(float time);

        #region IWorldObjectDelegate Members

        public void OnWorldObjectReleased(WorldObject obj)
        {
            controlledObject = null;
        }

        #endregion
    }
}
