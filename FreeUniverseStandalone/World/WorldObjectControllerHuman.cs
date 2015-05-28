using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeUniverse.Client;
using FreeUniverse.Net.Messages;

namespace FreeUniverse.World
{
    public class PlayerHUDFactory
    {
        public static PlayerHUD MakeHUD()
        {
            return new PlayerHUD();
        }
    }

    public class WorldObjectControllerHuman : WorldObjectController
    {
        private PlayerHUD hud { get; set; }

        public WorldObjectControllerHuman()
        {
            hud = PlayerHUDFactory.MakeHUD();
        }

        public override void SetWorldObject(WorldObject obj)
        {
            base.SetWorldObject(obj);
            hud.hudTarget = obj;
        }

        override public void Update(float time)
        {
            if (controlledObject == null)
                return;

            hud.Update(time);

            WorldObjectControlPanel panel = controlledObject.controlPanel;
            InputController input = ApplicationModel.inputController;
            WorldControllerClient worldController = controlledObject.worldController as WorldControllerClient;

            panel.movementControlsActive = input.userMovementControl;

            // Control engines
            if (input.userMovementControl)
                panel.SetEngineControlValues(
                    input.GetAxisControlForwardBackward(), input.GetAxisControlUpDown(), input.GetAxisControlLeftRight(),
                    input.normalizedClippedScreenMousePosition.x, input.normalizedClippedScreenMousePosition.y, input.GetAxisControlRoll());
            else
                panel.SetEngineControlValues(
                    input.GetAxisControlForwardBackward(), 0.0f, 0.0f,
                    0.0f, 0.0f, 0.0f);

            // fire guns
            if (input.FireAllActiveWeapons())
            {
                uint mask = panel.GetAllActiveWeaponsMask();

                if (mask != 0 )
                    worldController.FireWeapons(controlledObject.id, mask, panel.weaponAimPoint);
            }

            // Docking
            if (input.GetDockRequest() && hud.hudTarget != null)
            {
                DockReply reply = worldController.RequestDock(panel.worldObject.id, hud.selectedWorldObject.id);

                if (reply == DockReply.Ok)
                    worldController.Dock(panel.worldObject.id, hud.selectedWorldObject.id);
            }
        }

        Vector2 vecCenter = new Vector2(100.0f, 100.0f);

        public void OnGUI()
        {
            hud.OnGUI();
        }
    }
}
