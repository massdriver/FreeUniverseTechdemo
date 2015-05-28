using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.World.Zone
{
    public interface IZoneObserver
    {
        Vector3 zoneObserverPosition { get; }
        float zoneObserverVisibilityRange { get; }
        bool zoneObserverShouldCreateDust { get; }
    }
}
