using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeUniverse.Arch;

namespace FreeUniverse.World.Zone
{
    public class ZoneCubeElement
    {
        private GameObject element { get; set; }
        private GameObject visibleElement { get; set; }

        public ZoneCubeElement(ArchZoneElement arch, int layer, Vector3 pos, Vector3 scale, Quaternion rot)
        {
            element = Helpers.LoadUnityObject(arch.asset);

            if (element == null)
                return;

            Helpers.SetLayerDeep(element, layer);

            element.layer = layer;
            element.transform.localScale = scale;
            element.transform.rotation = rot;
            element.transform.position = pos;

            foreach (Transform child in element.transform.root)
            {

                if (child.name.StartsWith("hitbox_"))
                {
                    
                    MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
                    //meshCollider.convex = true;
                    child.gameObject.AddComponent<GameObjectUserData>().userData = this;
                    child.gameObject.renderer.enabled = false;
                }
                else
                {
                    visibleElement = child.gameObject;
                    //_visibleElement.renderer.castShadows = false;
                    //_visibleElement.renderer.receiveShadows = false;
                }
            }
        }

        public void Release()
        {
            if (element != null)
            {
                visibleElement = null;
                UnityEngine.Object.Destroy(element);
            }
        }

        //static float FADE_SPEED = 0.3f;
        //float _fade;

        public void Update(float time, bool visible)
        {
            // MH: do not change material color on fly
            /*
            if (visible)
                _fade += FADE_SPEED * time;
            else
                _fade -= FADE_SPEED * time;

            if (_fade > 1.0f)
                _fade = 1.0f;

            if (_fade < 0.0f)
                _fade = 0.0f;

            if (_visibleElement != null)
            {
                Color c = _visibleElement.renderer.material.color;
                c.a = _fade;
                c.b = _fade;
                c.g = _fade;
                c.r = _fade;
                _visibleElement.renderer.material.color = c;
            }

            if (_element.renderer != null)
            {
                Color c = _element.renderer.material.color;
                c.a = _fade;
                c.b = _fade;
                c.g = _fade;
                c.r = _fade;
                _element.renderer.material.color = c;
            }
             */
        }
    }
}
