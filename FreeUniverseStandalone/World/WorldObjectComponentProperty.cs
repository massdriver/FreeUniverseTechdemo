using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;
using FreeUniverse.Common;

namespace FreeUniverse.World
{
    public class WorldObjectComponentProperty
    {
        public bool activated { get; protected set; }
        public WorldObjectComponent parentComponent { get; protected set; }
        public ComponentPropertyType type { get; protected set; }

        public WorldObjectComponentProperty(WorldObjectComponent parent, ArchWorldObjectComponentProperty arch)
        {
            parentComponent = parent;
            parentComponent.indexProperties.Insert(this);
        }

        public virtual bool Activate(bool flag)
        {
            activated = flag;
            return activated;
        }

        public virtual void Update(float time)
        {

        }

        public virtual void PostInit()
        {

        }

        public virtual void Release()
        {
            parentComponent.indexProperties.Remove(this);
        }
    }
}
