using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;

namespace FreeUniverse.World
{
    public class BaseContents
    {
        public ArchObjectBaseContents arch { get; set; }

        public BaseContents()
        {

        }

        public BaseContents(ArchObjectBaseContents arch)
        {
            this.arch = arch;
        }

    }
}
