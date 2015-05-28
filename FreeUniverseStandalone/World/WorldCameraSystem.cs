
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;
using UnityEngine;

namespace FreeUniverse.World
{
    public class WorldCameraSystem : IWorldObjectDelegate
    {
        public Camera camera { get { return _systemCamera.gameObject.camera; } }

        WorldCamera _systemCamera;
        WorldCamera _backgroundCamera;
        WorldCameraSystemBehaviour _currentBehaviour;
        WorldCameraSystemBehaviourThirdPerson _behaviourThirdPerson;

        public Transform GetTransform()
        {
            return _systemCamera.gameObject.transform;
        }

        public WorldCameraSystem(int layer, int background)
        {
            //_object.name = "World Camera System";

            _systemCamera = new WorldCamera();
            _systemCamera.gameObject.camera.cullingMask = (1 << layer);
            _systemCamera.gameObject.layer = layer;
            _systemCamera.gameObject.camera.clearFlags = CameraClearFlags.Depth; // To draw properly over background
            _systemCamera.gameObject.camera.depth = 10.0f;
            _systemCamera.gameObject.camera.hdr = false;
            _systemCamera.gameObject.camera.far = 2000000.0f;
            //_systemCamera.gameObject.transform.parent = this.gameObject.transform;

            // add audio listener
            _systemCamera.gameObject.AddComponent<AudioListener>();

            // Add post effects to system camera since it will be last one in rendering path

            _backgroundCamera = new WorldCamera();
            _backgroundCamera.gameObject.layer = background;
            _backgroundCamera.gameObject.camera.cullingMask = (1 << background);
            _backgroundCamera.gameObject.camera.clearFlags = CameraClearFlags.Color;
            _backgroundCamera.gameObject.camera.backgroundColor = Color.black;
            _backgroundCamera.gameObject.camera.hdr = false;
            //_backgroundCamera.gameObject.transform.parent = this.gameObject.transform;

            _behaviourThirdPerson = new WorldCameraSystemBehaviourThirdPerson(this);
        }

        public void ApplySystemParameters(ArchObjectSystem system)
        {
            if( null != _backgroundCamera )
                _backgroundCamera.gameObject.camera.backgroundColor = new Color(system.backgroundColor.x,system.backgroundColor.y,system.backgroundColor.z);
        }

        public void FollowThirdPerson(WorldObject obj)
        {
            _behaviourThirdPerson.target = obj;
            _currentBehaviour = _behaviourThirdPerson;
            _behaviourThirdPerson.Activate();
        }

        public void DisableCameraBehaviour()
        {
            if (_currentBehaviour == null) return;

            _currentBehaviour.Deactivate();
            _currentBehaviour = null;
        }

        public void OnGUI()
        {
            if (_currentBehaviour != null)
                _currentBehaviour.OnGUI();
        }

        public void Update(float time)
        {
            if (_currentBehaviour != null)
                _currentBehaviour.Update(time);

            // Sync rotation of background camera with main camera
            if( null != _backgroundCamera )
                _backgroundCamera.gameObject.transform.rotation = _systemCamera.gameObject.camera.transform.rotation;
            
        }

        #region IWorldObjectDelegate Members

        public void OnWorldObjectReleased(WorldObject obj)
        {
            
        }

        #endregion
    }

}
