using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Common;

namespace FreeUniverse.Arch
{
    public class ArchWorldObjectComponentProperty : ArchObject
    {
        protected ComponentPropertyType _type = ComponentPropertyType.Null;
        public ComponentPropertyType type { get { return _type; } }
    }
}
