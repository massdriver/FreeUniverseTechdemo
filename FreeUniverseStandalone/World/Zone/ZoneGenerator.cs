using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeUniverse.Arch;
using FreeUniverse.World;
using FreeUniverse.Core;

namespace FreeUniverse.World.Zone
{
    public class ZoneGenerator
    {
        private ZoneVolumeProvider volumeProvider { get; set; }
        private ArchObjectSystemZone zoneParams { get; set; }
        private WorldController worldController { get; set; }
        private ListEx<ZoneCube> zoneCubes { get; set; }
        private List<IZoneObserver> zoneObservers { get; set; }
        private ArchZone zoneArch { get; set; }
        private List<ZoneCubeElement> outerElements { get; set; }
        private Dictionary<uint, ZoneCube> zoneCubesHashed { get; set; }
        private ListEx<ZoneDustGenerator> dust { get; set; }
        private bool isServerZoneGenerator { get; set; }
        private int cubeCount { get; set; }
        private int elementCount { get; set; }

        public ZoneGenerator(ArchObjectSystemZone zoneParams, WorldController worldController)
        {
            zoneCubes = new ListEx<ZoneCube>();
            outerElements = new List<ZoneCubeElement>();
            zoneCubesHashed = new Dictionary<uint, ZoneCube>();
            dust = new ListEx<ZoneDustGenerator>();

            isServerZoneGenerator = worldController is WorldControllerServer;

            this.zoneParams = zoneParams;
            this.worldController = worldController;
            zoneArch = ArchModel.GetZone(zoneParams.zoneArch);
            volumeProvider = CreateZoneVolumeProvider(zoneParams.shapeType);
            zoneObservers = worldController.zoneObservers;

            CreateOuterElements();
            Update(0.0f); // just to ensure that on start everything is already generated
        }

        public void Release()
        {
            volumeProvider = null;
            zoneParams = null;
            worldController = null;

            {
                ZoneCube cube = zoneCubes.Iterate();
                while (cube != null)
                {
                    cube.Release();
                    cube = zoneCubes.Next();
                }

                zoneCubes = null;
            }

            zoneObservers.Clear();
            zoneObservers = null;

            zoneArch = null;

            zoneCubesHashed.Clear();
            zoneCubesHashed = null;

            {
                ZoneDustGenerator de = dust.Iterate();
                while (de != null)
                {
                    de.FreeAllDust();
                    de = dust.Next();
                }

                dust.Clear();
                dust = null;
            }

            foreach (ZoneCubeElement e in outerElements)
                e.Release();

            outerElements.Clear();
            outerElements = null;
        }

        public void OnVisualObserverChanged()
        {
            if (dust.count != 0)
            {
                foreach( ZoneDustGenerator e in dust)
                    e.FreeAllDust();

                dust.Clear();
            }

            if (worldController.visualObserver != null)
            {
                foreach (ArchZoneDust e in zoneArch.dust)
                    dust.Add(new ZoneDustGenerator(e, worldController.visualObserver));
            }
        }

        private void CreateOuterElement(Vector3 pos, ArchZoneElement arch)
        {
            Vector3 offset = pos;// _zoneParams.position + UnityEngine.Random.insideUnitSphere * _zoneParams.size.magnitude;

            Vector3 scale;
            if (arch.uniformScale)
            {
                float uniformScale = zoneParams.size.magnitude / 2.0f;// *5.0f;
                scale.x = uniformScale;
                scale.y = uniformScale;
                scale.z = uniformScale;
            }
            else
                scale = new Vector3(
                    UnityEngine.Random.Range(arch.minScale.x, arch.maxScale.x),
                    UnityEngine.Random.Range(arch.minScale.y, arch.maxScale.y),
                    UnityEngine.Random.Range(arch.minScale.z, arch.maxScale.z));

            Quaternion rot = UnityEngine.Random.rotation;

            outerElements.Add(new ZoneCubeElement(arch, worldController.layer, offset, scale, rot));
        }

        private void CreateOuterElements()
        {
            if (isServerZoneGenerator) return;

            if (zoneArch.outerElements.Count == 0)
                return;

            UnityEngine.Random.seed = (int)Noise.Noise3D(zoneParams.position);

            List<Vector3> outerPoints = volumeProvider.CreatePointsInside(zoneParams.size.magnitude/4.0f);

            foreach (Vector3 v in outerPoints)
            {
                if( Noise.Noise3D(v) > 0.5f )
                    CreateOuterElement(v, zoneArch.outerElements[zoneArch.outerElements.Count - 1]);
            }
        }

        private ZoneVolumeProvider CreateZoneVolumeProvider(ZoneShapeType type)
        {
            if (type == ZoneShapeType.Sphere)
                return new ZoneVolumeProviderSphere(zoneParams);

            return new ZoneVolumeProviderSphere(zoneParams);
        }

        public void Update(float time)
        {
            /*
            foreach (IZoneObserver e in zoneObservers)
                CreateCubesForObserver(e);

            foreach (ZoneDustGenerator e in dust)
                e.Update(time);

            ZoneCube cube = zoneCubes.Iterate();
            while (cube != null)
            {
                cube.Update(time);
                
                if (cube.CanBeRemoved())
                {
                    cube.Release();
                    zoneCubesHashed.Remove(Helpers.Hash3D(cube.center, (uint)zoneArch.cubeSize));
                    zoneCubes.Remove();
                }

                cube = zoneCubes.Next();
            }

            cubeCount = zoneCubes.count;
            */
        }

        private ZoneCube FindByCenter(Vector3 center)
        {
            ZoneCube e;
            zoneCubesHashed.TryGetValue(Helpers.Hash3D(center, (uint)zoneArch.cubeSize), out e);
            return e;
        }

        private void CreateCubesForObserver(IZoneObserver observer)
        {
            Vector3 cutPosition = Helpers.CutToNearestIntegerValue(observer.zoneObserverPosition, zoneArch.cubeSize);
            int cubesPerAxis = 1 + (int)((observer.zoneObserverVisibilityRange - zoneArch.cubeSize * 0.5f) / zoneArch.cubeSize);

            for (int i = -cubesPerAxis + 1; i <= cubesPerAxis - 1; i++)
            {
                for (int j = -cubesPerAxis + 1; j <= cubesPerAxis - 1; j++)
                {
                    for (int k = -cubesPerAxis + 1; k <= cubesPerAxis - 1; k++)
                    {
                        Vector3 offset = new Vector3( zoneArch.cubeSize * i,  zoneArch.cubeSize * j, zoneArch.cubeSize * k);
                        Vector3 cubeCenter = cutPosition + offset;

                        // Cube already exists
                        if (FindByCenter(cubeCenter) != null)
                            continue;

                        // Exclude cube if it doesnt belong to zone volume
                        if (volumeProvider.GetDistance(cubeCenter) > 0.0f)
                            continue;

                        if ((cubeCenter - observer.zoneObserverPosition).sqrMagnitude > (observer.zoneObserverVisibilityRange * observer.zoneObserverVisibilityRange))
                            continue;

                        ZoneCube cube = new ZoneCube(cubeCenter, volumeProvider, zoneArch, zoneObservers, worldController.layer);
                        cube.isServerZoneCube = isServerZoneGenerator;
                        zoneCubes.Add(cube);
                        zoneCubesHashed.Add(Helpers.Hash3D(cubeCenter, (uint)zoneArch.cubeSize), cube);
                    }
                }
            }
        }

        public void OnGUI()
        {
            //GUI.Label(Helpers.MakeRect(new Vector3(150.0f, 50.0f), 300.0f, 25.0f), "Cubes = " + _cubeCount + ", Elements = " + _elementCount );
        }
    }
}
