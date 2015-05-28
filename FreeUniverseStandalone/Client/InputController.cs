using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Client
{
    public class InputController
    {
        public bool FireAllActiveWeapons()
        {
            return Input.GetMouseButton(1);
        }

        public bool ShouldDrawHUD()
        {
            return true;
        }

        public float GetAxisControlPitch()
        {
            return Input.GetAxis("Mouse Y");
        }

        public float GetAxisControlYaw()
        {
            return Input.GetAxis("Mouse X");
        }

        public float GetAxisControlRoll()
        {
            if (Input.GetKey(KeyCode.Q))
                return 1.0f;
            else
                if (Input.GetKey(KeyCode.E))
                    return -1.0f;

            return 0.0f;
        }

        public float GetAxisControlForwardBackward()
        {
            if (Input.GetKey(KeyCode.W))
                return 1.0f;
            else
            if (Input.GetKey(KeyCode.S))
                return -1.0f;

            return 0.0f;
        }

        public float GetAxisControlLeftRight()
        {

            if (Input.GetKey(KeyCode.D))
                return -1.0f;
            else
                if (Input.GetKey(KeyCode.A))
                    return 1.0f;

            return 0.0f;
        }

        public float GetAxisControlUpDown()
        {
            if (Input.GetKey(KeyCode.R))
                return 1.0f;
            else
                if (Input.GetKey(KeyCode.F))
                    return -1.0f;

            return 0.0f;
        }

        public bool GetDockRequest()
        {
            return Input.GetKeyUp(KeyCode.F3);
        }

        bool _userMovementControl = false;
        public bool userMovementControl { get { return _userMovementControl; } }

        Vector2 _normalizedScreenMousePosition = new Vector2(0.0f, 0.0f);
        public Vector2 normalizedScreenMousePosition { get { return _normalizedScreenMousePosition; } }

        Vector2 _normalizedClippedScreenMousePosition = new Vector2(0.0f, 0.0f);
        public Vector2 normalizedClippedScreenMousePosition { get { return _normalizedClippedScreenMousePosition; } }

        public Vector3 viewportMousePosition { get; private set; }

        void UpdateViewportMousePosition()
        {
            Vector3 mpos = Input.mousePosition;
            viewportMousePosition = new Vector3(mpos.x / Screen.width, mpos.y / Screen.height, 0.0f);
        }

        void CalcNormalizedClippedMousePosition()
        {
            Vector3 mpos = Input.mousePosition;

            float x = 2.0f * mpos.x / Screen.width - 1.0f;
            float y = 2.0f * mpos.y / Screen.height - 1.0f;

            if (x > 0.5f) x = 0.5f;
            if (y > 0.5f) y = 0.5f;
            if (x < -0.5f) x = -0.5f;
            if (y < -0.5f) y = -0.5f;

            x *= 2.0f;
            y *= 2.0f;

            _normalizedClippedScreenMousePosition.x = x;
            _normalizedClippedScreenMousePosition.y = y;
        }

        public void Update(float time)
        {
            float scx = Screen.width / 2.0f;
            float scy = Screen.height / 2.0f;

            _normalizedScreenMousePosition.x = (Input.mousePosition.x - scx) / Screen.width;
            _normalizedScreenMousePosition.y = (Input.mousePosition.y - scy) / Screen.height;

            CalcNormalizedClippedMousePosition();

            if (Input.GetKeyUp(KeyCode.Space))
                _userMovementControl = !_userMovementControl;

            UpdateViewportMousePosition();
        }
    }
}
