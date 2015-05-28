using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Core
{
    public interface IReleasable
    {
        void Release();
        bool CanBeReleased();
    }
}
