using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;
using UnityEngine;
using FreeUniverse.Client;

namespace FreeUniverse.World
{
    class WorldCameraSystemBehaviourThirdPerson : WorldCameraSystemBehaviour, IWorldObjectDelegate
    {
        WorldObject _target;
        WorldObjectComponentHardpoint _hpNear;
        WorldObjectComponentHardpoint _hpFar;
        CameraViewProperties _cameraProperties;

        public WorldObject target
        {
            get { return _target; }

            set
            {
                if (_target != null) _target.RemoveDelegate(this);

                _target = value;

                if (_target == null) return;

                _target.AddDelegate(this);
                _hpNear = _target.GetRootHullHardpoint("hp_camera_near");
                _hpFar = _target.GetRootHullHardpoint("hp_camera_far");
                _cameraProperties = _target.root.hull.arch.cameraProperties;
                _state = ThirdPersonCameraState.Near;
                _lerpSteps = 0.0f;
            }
        }

        public WorldCameraSystemBehaviourThirdPerson(WorldCameraSystem camera)
            : base(camera)
        {

        }

        public override void Activate()
        {

        }

        public override void Deactivate()
        {
            this.target = null; // use this to resign delegate
            _hpFar = null;
            _hpNear = null;
        }

        public override void OnGUI()
        {
        }

        enum ThirdPersonCameraState
        {
            Null, Far, Near, FarToNear, NearToFar
        };

        ThirdPersonCameraState _state = ThirdPersonCameraState.Null;
        Vector2 _pos2offset = new Vector2();
        static Vector2 VEC2_NULL = new Vector2();
        float _lerpSteps = 0.0f;

        public override void Update(float time)
        {
            if (_target == null)
                return;

            if (_hpNear == null || _hpFar == null)
                return;

            Transform cameraTransform = _cameraSystem.GetTransform();
            InputController input = ApplicationModel.inputController;
            Vector2 vecMouse = input.normalizedClippedScreenMousePosition;
            bool interactive = _target.controlPanel.movementControlsActive;
            WorldObjectControlPanel panel = _target.controlPanel;

            float transitionSpeed = _cameraProperties.offset.y;
            float maxLength = _cameraProperties.offset.x;

            Vector2 maxOffset = new Vector2(maxLength * vecMouse.x, vecMouse.y * maxLength);

            if (_target.controlPanel.movementControlsActive)
                _pos2offset = Vector2.Lerp(_pos2offset, maxOffset, transitionSpeed * time);
            else
                _pos2offset = Vector2.Lerp(_pos2offset, VEC2_NULL, transitionSpeed * time);

            if (interactive && (panel.thrustValueForwardBackward > 0.0f))
            {
                if (_state == ThirdPersonCameraState.Near)
                {
                    _state = ThirdPersonCameraState.NearToFar;
                    _lerpSteps = 0.0f;
                }
                else
                    if (_state == ThirdPersonCameraState.Far)
                        _state = ThirdPersonCameraState.Far;
                    else if (_state == ThirdPersonCameraState.FarToNear)
                    {
                        _state = ThirdPersonCameraState.NearToFar;
                        _lerpSteps = 1.0f - _lerpSteps;
                    };
            }
            else
            {
                if (_state == ThirdPersonCameraState.Near)
                    _state = ThirdPersonCameraState.Near;
                else if (_state == ThirdPersonCameraState.Far)
                {
                    _state = ThirdPersonCameraState.FarToNear;
                    _lerpSteps = 0.0f;
                }
                else if (_state == ThirdPersonCameraState.NearToFar)
                {
                    _state = ThirdPersonCameraState.FarToNear;
                    _lerpSteps = 1.0f - _lerpSteps;
                };
            };

            if (_state == ThirdPersonCameraState.NearToFar)
            {
                _lerpSteps += time * transitionSpeed;

                Transform hpPos1 = _hpNear.transform;
                Transform hpPos2 = _hpFar.transform;
                Vector3 lerpedPos = Vector3.Lerp(hpPos1.position, hpPos2.position, _lerpSteps);
                cameraTransform.position = Helpers.CopyVector3(lerpedPos);
                cameraTransform.Translate(new Vector3(_pos2offset.x, _pos2offset.y, 0.0f), Space.Self);
                cameraTransform.rotation = Helpers.GetRotationQuaternion(hpPos2);

                if (_lerpSteps >= 1.0f)
                {
                    _state = ThirdPersonCameraState.Far;
                    _lerpSteps = 0.0f;
                };
            }
            else if (_state == ThirdPersonCameraState.FarToNear)
            {
                _lerpSteps += time * transitionSpeed * 0.5f;

                Transform hpPos1 = _hpFar.transform;
                Transform hpPos2 = _hpNear.transform;
                Vector3 lerpedPos = Vector3.Lerp(hpPos1.position, hpPos2.position, _lerpSteps);
                cameraTransform.position = Helpers.CopyVector3(lerpedPos);
                cameraTransform.Translate(new Vector3(_pos2offset.x, _pos2offset.y, 0.0f), Space.Self);
                cameraTransform.rotation = Helpers.GetRotationQuaternion(hpPos2);

                if (_lerpSteps >= 1.0f)
                {
                    _state = ThirdPersonCameraState.Near;
                    _lerpSteps = 0.0f;
                };
            }
            else if (_state == ThirdPersonCameraState.Near)
            {
                cameraTransform.position = Helpers.CopyVector3(_hpNear.transform.position);
                cameraTransform.Translate(new Vector3(_pos2offset.x, _pos2offset.y, 0.0f), Space.Self);
                cameraTransform.rotation = Helpers.GetRotationQuaternion(_hpNear.transform);
            }
            else if (_state == ThirdPersonCameraState.Far)
            {
                cameraTransform.position = Helpers.CopyVector3(_hpFar.transform.position);
                cameraTransform.Translate(new Vector3(_pos2offset.x, _pos2offset.y, 0.0f), Space.Self);
                cameraTransform.rotation = Helpers.GetRotationQuaternion(_hpFar.transform);
            }

            // update object's weapon aim point
            {
                float projectDistance = panel.targetPointCrossDistance;
                Vector2 nm = input.normalizedScreenMousePosition + (new Vector2(0.5f, 0.5f));
                _target.controlPanel.weaponAimPoint = _cameraSystem.camera.ViewportToWorldPoint(new Vector3(nm.x, nm.y, projectDistance));
            }


        }

        #region IWorldObjectDelegate Members

        public void OnWorldObjectReleased(WorldObject obj)
        {
            if (obj == _target)
                _target = null;
        }

        #endregion
    }
}
