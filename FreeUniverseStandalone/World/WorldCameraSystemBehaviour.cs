using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;
using UnityEngine;

namespace FreeUniverse.World
{
    public abstract class WorldCameraSystemBehaviour
    {
        public WorldCameraSystemBehaviour(WorldCameraSystem camera)
        {
            _cameraSystem = camera;
        }

        protected WorldCameraSystem _cameraSystem;

        public abstract void Activate();
        public abstract void Deactivate();
        public abstract void Update(float time);
        public abstract void OnGUI();
    }
}