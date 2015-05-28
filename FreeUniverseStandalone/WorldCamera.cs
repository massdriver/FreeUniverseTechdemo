using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeUniverse;

namespace FreeUniverse.World
{
    public class WorldCamera : BaseGameObject
    {
        public WorldCamera()
        {
            _object.AddComponent("Camera");
            _object.camera.renderingPath = RenderingPath.DeferredLighting;
            _object.camera.backgroundColor = Color.black;
            _object.name = "Camera";
        }

        override public void OnGUI()
        {
           
        }

        override public void OnUpdate()
        {

        }
    }
}
