using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreeUniverse.Arch;
using UnityEngine;
using FreeUniverse.Common;

namespace FreeUniverse.World
{
    public class WorldObjectComponentPropertyEngine : WorldObjectComponentProperty
    {
        // original arch
        public ArchWorldObjectComponentPropertyEngine arch { get; private set; }

        public WorldObjectComponentPropertyEngine(WorldObjectComponent parent, ArchWorldObjectComponentPropertyEngine arch)
            : base(parent, arch)
        {
            this.arch = arch;
            type = ComponentPropertyType.Engine;      
        }

        // control values are in range of -1.0f ... +1.0f
        public float thrustValueForwardBackward { get; set; }
        public float thrustValueUpDown { get; set; }
        public float thrustValueLeftRight { get; set; }
        public float torqueValueYaw { get; set; }
        public float torqueValuePitch { get; set; }
        public float torqueValueRoll { get; set; }
        public float currentHeat { get; set; }

        public void BoostForward()
        {

        }

        public void SetControlValues(
            float thrustValueForwardBackward, float thrustValueUpDown, float thrustValueLeftRight,
            float torqueValueYaw, float torqueValuePitch, float torqueValueRoll )
        {
            this.thrustValueForwardBackward = thrustValueForwardBackward;
            this.thrustValueUpDown = thrustValueUpDown;
            this.thrustValueLeftRight = thrustValueLeftRight;
            this.torqueValueYaw = torqueValueYaw;
            this.torqueValuePitch = torqueValuePitch;
            this.torqueValueRoll = torqueValueRoll;
        }

        Rigidbody GetHullRigidBody()
        {
            if (parentComponent.hull == null)
                return null;

            if (parentComponent.hull.worldRigidBody == null)
                return null;

            return parentComponent.hull.worldRigidBody.rigidBody;
        }

        public override void Update(float time)
        {
            // Apply control values to obtain propulsion
            if (activated)
            {
                Rigidbody body = GetHullRigidBody();
   
                if (body != null)
                {

                    if ((thrustValueLeftRight != 0.0f) || (thrustValueUpDown != 0.0f) || (thrustValueForwardBackward != 0.0f))
                        body.AddRelativeForce(
                        -time * arch.forceThrustLeftRight * thrustValueLeftRight,
                        time * arch.forceThrustUpDown * thrustValueUpDown,
                        time * arch.forceThrustForwardBackward * thrustValueForwardBackward
                        );


                    if ((torqueValuePitch != 0.0f) || (torqueValueYaw != 0.0f) || (torqueValueRoll != 0.0f))
                        body.AddRelativeTorque(      
                            -time * torqueValuePitch * arch.torquePitch,
                            time * torqueValueYaw * arch.torqueYaw,
                            time * torqueValueRoll * arch.torqueRoll);
                    
                }
            }
        }

    }
}
