using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeUniverse.Core;
using FreeUniverse.Arch;

namespace FreeUniverse.World.Zone
{
    public class ZoneCube
    {
        public const float ZONE_CUBE_LIFETIME = 5.0f;
        public const float NOISE_MOD = 100000.0f;
        public Vector3 center { get; private set; }
        public ZoneVolumeProvider volumeProvider { get; private set; }
        public float noise { get; set; }
        public bool elementsCreated { get; private set; }
        public ArchZone zoneArch { get; private set; }
        public int layer { get; private set; }
        public bool isServerZoneCube { get; set; }
        public bool isVisible { get; private set; }
        private List<IZoneObserver> observers { get; set; }
        private ListEx<ZoneCubeElement> elements { get; set; }
        private float removeLifeTime { get; set; }

        public ZoneCube(Vector3 center, ZoneVolumeProvider volumeProvider, ArchZone zoneArch, List<IZoneObserver> observers, int layer)
        {
            removeLifeTime = ZONE_CUBE_LIFETIME;
            elements = new ListEx<ZoneCubeElement>();

            this.observers = observers;
            this.zoneArch = zoneArch;
            this.center = center;
            this.volumeProvider = volumeProvider;
            this.noise = Noise.Noise3D(center) * NOISE_MOD;
            this.layer = layer;
        }

        public int elementCount { get { return elements.count; } }

        public bool CanBeRemoved()
        {
            return removeLifeTime < 0.0f;
        }

        private bool IsVisible()
        {
            foreach (IZoneObserver e in observers)
            {
                isVisible = (e.zoneObserverPosition - center).sqrMagnitude <= e.zoneObserverVisibilityRange;

                if (isVisible)
                    return true;
            }

            return false;
        }

        public void Release()
        {
            ZoneCubeElement element = elements.Iterate();

            while (element != null)
            {
                element.Release();
                element = elements.Next();
            }

            elements.Clear();
        }

        public void CreateElements()
        {
            elementsCreated = true;

            // Reset unity random seed
            UnityEngine.Random.seed = (int)noise;

            foreach (ArchZoneElement e in zoneArch.innerElements)
            {
                if (isServerZoneCube && !e.serverShouldGenerate) continue;

                if (e.chance < 1.0f)
                {
                    float chance = UnityEngine.Random.Range(0.0f, 1.0f);

                    if( chance > e.chance )
                        continue;
                }

                int count = UnityEngine.Random.Range(e.minCount, e.maxCount);

                for (int i = 0; i < count; i++)
                {

                    Vector3 offset = center + UnityEngine.Random.insideUnitSphere * zoneArch.cubeSize * 0.5f;

                    Vector3 scale;
                    if( e.uniformScale )
                    {
                        float uniformScale = UnityEngine.Random.Range(e.minScale.x, e.maxScale.x);
                        scale.x = uniformScale;
                        scale.y = uniformScale;
                        scale.z = uniformScale;
                    }
                    else
                        scale = new Vector3( UnityEngine.Random.Range(e.minScale.x, e.maxScale.x), UnityEngine.Random.Range(e.minScale.y, e.maxScale.y), UnityEngine.Random.Range(e.minScale.z, e.maxScale.z));

                    Quaternion rot = UnityEngine.Random.rotation;

                    elements.Add( new ZoneCubeElement(e, layer, offset, scale, rot) );
                }
            }
        }

        public void Update(float time)
        {

            if (!elementsCreated)
                CreateElements();

            if (IsVisible())
            {
                removeLifeTime = ZONE_CUBE_LIFETIME;
            }

            ZoneCubeElement element = elements.Iterate();

            while (element != null)
            {
                element.Update(time, isVisible);
                element = elements.Next();
            }

            removeLifeTime -= time;
        }
    }
}
