using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeUniverse.Arch;

namespace FreeUniverse.World.Zone
{
    public class ZoneVolumeProviderSphere : ZoneVolumeProvider
    {
        float _radius;
        Vector3 _position;

        public ZoneVolumeProviderSphere(ArchObjectSystemZone zoneArch) : base(zoneArch)
        {
            _position = zoneArch.position;
            _radius = zoneArch.size.x;
        }

        public override float GetDistance(Vector3 point)
        {
            float mag = Helpers.FastLength(point - _position);//Vector3.Magnitude(point - _position);

            if (mag < _radius)
                return -mag;

            return mag;
        }

        public override List<Vector3> CreatePointsInside(float stepSize)
        {
            List<Vector3> lst = new List<Vector3>();
            int pointsPerAxis = (int)((_radius * 2.0f) / stepSize);

            for (int i = -pointsPerAxis + 1; i <= pointsPerAxis - 1; i++)
            {
                for (int j = -pointsPerAxis + 1; j <= pointsPerAxis - 1; j++)
                {
                    for (int k = -pointsPerAxis + 1; k <= pointsPerAxis - 1; k++)
                    {
                        Vector3 point = new Vector3( i * stepSize, j * stepSize, k * stepSize );
                        point += _position;

                        if( GetDistance(point) > 0.0f )
                            continue;

                        lst.Add(point);
                    }
                }
            }

            return lst;
        }
    }
}
