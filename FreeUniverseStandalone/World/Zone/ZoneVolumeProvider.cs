using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;
using UnityEngine;

namespace FreeUniverse.World.Zone
{
    public abstract class ZoneVolumeProvider
    {
        protected ArchObjectSystemZone _zoneArch;

        public ZoneVolumeProvider(ArchObjectSystemZone zoneArch)
        {
            _zoneArch = zoneArch;
        }

        public abstract float GetDistance(Vector3 point);
        public abstract List<Vector3> CreatePointsInside(float stepSize);
    }
}
