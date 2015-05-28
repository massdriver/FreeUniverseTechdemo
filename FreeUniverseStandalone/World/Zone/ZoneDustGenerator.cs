using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;
using UnityEngine;
using FreeUniverse.Core;

namespace FreeUniverse.World.Zone
{
    public class DustParticle
    {
        GameObject _element;

        public bool IsVisible(Vector3 center, float dist)
        {
            if ((center - _element.transform.position).sqrMagnitude < (dist * dist))
                return true;

            return false;
        }

        public DustParticle(float mass, string asset, Vector3 pos, Quaternion rot, Vector3 scale, Vector3 velocity)
        {

            _element = Helpers.LoadUnityObject(asset);

            if (_element == null)
                return;

            _element.layer = ConstData.LAYER_CLIENT_WORLD;
            _element.transform.localScale = scale;
            _element.transform.rotation = rot;
            _element.transform.position = pos;

            if (velocity.sqrMagnitude > 0.0f)
            {
                Rigidbody body = _element.AddComponent<Rigidbody>();
                body.mass = mass;
                body.useGravity = false;
                body.velocity = velocity;
                body.drag = 0.0f;
                body.angularDrag = 0.5f;
            }

            foreach (Transform child in _element.transform.root)
            {
                child.gameObject.layer = _element.layer;

                if (child.name.StartsWith("hitbox_"))
                {

                    MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
                    //meshCollider.convex = true;
                    child.gameObject.AddComponent<GameObjectUserData>().userData = this;
                    child.gameObject.renderer.enabled = false;
                }
                else
                {
                    //_visibleElement = child.gameObject;
                    //_visibleElement.renderer.castShadows = false;
                    //_visibleElement.renderer.receiveShadows = false;
                }
            }
        }

        public void Update(float time)
        {

        }

        public bool CanBeReleased()
        {
            return false;
        }

        public void Release()
        {
            if (_element != null)
                UnityEngine.Object.Destroy(_element);
        }
    }

    public class ZoneDustGenerator
    {
        private IZoneObserver observer { get; set; }
        private ArchZoneDust arch { get; set; }
        private IndexArray<DustParticle> dustItems { get; set; }

        public ZoneDustGenerator(ArchZoneDust arch, IZoneObserver observer)
        {
            dustItems = new IndexArray<DustParticle>(256);
            this.observer = observer;
            this.arch = arch;
        }

        public void FreeAllDust()
        {
            foreach (DustParticle p in dustItems)
                p.Release();

            dustItems.Clear();

            observer = null;
            dustItems = null;
            arch = null;
        }

        public void Update(float time)
        {

            foreach (DustParticle p in dustItems)
            {
                p.Update(time);

                if (!p.IsVisible(observer.zoneObserverPosition, arch.visibilityRange))
                {
                    p.Release();
                    dustItems.RemoveAtIterator();
                }
            }


            // create new
            {
                int maxCount = dustItems.Capacity >= arch.maxCount ? arch.maxCount : dustItems.Capacity;

                while (dustItems.Count < maxCount)
                {
                    {
                        Vector3 pos = UnityEngine.Random.onUnitSphere * arch.visibilityRange * 0.5f;
                        Vector3 velocity = UnityEngine.Random.insideUnitSphere * arch.randomVelocity;
                        dustItems.Insert(
                            new DustParticle(
                                arch.mass,
                                arch.asset,
                                pos + observer.zoneObserverPosition,
                                UnityEngine.Random.rotation,
                                arch.minScale,
                                velocity)
                                );
                    }
                }
            }
        }
    }
}
