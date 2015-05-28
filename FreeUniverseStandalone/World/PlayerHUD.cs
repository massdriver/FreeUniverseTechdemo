using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;
using FreeUniverse.GUILib;

namespace FreeUniverse.World
{
    public class PlayerHUD : IWorldObjectDelegate
    {
        public Camera visibleCamera { get; set; }
        public WorldObjectComponent selectedComponent { get; set; }
        public WorldObject selectedWorldObject
        {
            get
            {
                if (selectedComponent == null) return null;

                return selectedComponent.parentWorldObject;
            }
        }

        WorldObject _target;
        public WorldObject hudTarget
        {
            get { return _target; }
            set
            {
                if (_target != null)
                    _target.RemoveDelegate(this);

                _target = value;

                if (_target != null)
                    _target.AddDelegate(this);
            }
        }

        public PlayerHUD()
        {
            
        }

        public virtual void Update(float time)
        {
            if (_target == null) return;
        }

        public virtual void OnGUI()
        {
            if ((_target == null) || !ApplicationModel.inputController.ShouldDrawHUD())
                return;

            GUI.skin = GUIExtensions.SKIN_DEFAULT;

            // Draw movement control status text at the bottom of screen
            {
                if (_target.controlPanel.movementControlsActive)
                {
                    GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                    GUI.Label(Helpers.MakeRect(new Vector2(Screen.width / 2.0f, Screen.height - 20.0f), 200.0f, 50.0f), "Movement controls active");
                }
            }

            WorldObjectStats stats = _target.controlPanel.GetWorldObjectBaseStats();

            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUI.Label(Helpers.MakeRect(new Vector2(100.0f, Screen.height - 100.0f + 15.0f * 4.0f), 400.0f, 25.0f), "Pos: " + _target.rootHullPosition.ToString() );

            float offset = 200.0f;

            DrawBar(GUIBarType.Left, stats.speed, stats.maxSpeed, new Vector2(Screen.width / 2.0f - offset, Screen.height / 2.0f), Color.red);

            DrawUnderbar(GUIBarType.UnderbarLeft,
                new Vector2(Screen.width / 2.0f - offset, Screen.height / 2.0f) +
                new Vector2(-22.0f, 3.0f - GUIExtensions.texture_bar_mask_left.height / 2.0f),
                "Speed:", stats.speed, stats.maxSpeed, false);

            DrawBar(GUIBarType.Left, 0.0f, stats.maxSpeed, new Vector2(Screen.width / 2.0f - offset + 10.0f, Screen.height / 2.0f), Color.yellow);

            DrawUnderbar(GUIBarType.UnderbarLeft,
                new Vector2(Screen.width / 2.0f - offset, Screen.height / 2.0f) +
                new Vector2(-25.0f, 3.0f - GUIExtensions.texture_bar_mask_left.height / 4.0f),
                "Energy:", 0.0f, stats.maxSpeed, false);

            DrawBar(GUIBarType.Right, 0.0f, stats.maxSpeed, new Vector2(Screen.width / 2.0f + offset - 10.0f, Screen.height / 2.0f), Color.blue);

            DrawUnderbar(GUIBarType.UnderbarRight,
                new Vector2(Screen.width / 2.0f + offset, Screen.height / 2.0f) +
                new Vector2(25.0f, 3.0f - GUIExtensions.texture_bar_mask_left.height / 4.0f),
                "Shield:", 0.0f, stats.maxSpeed, false);

            DrawBar(GUIBarType.Right, stats.hull, stats.maxHull, new Vector2(Screen.width / 2.0f + offset, Screen.height / 2.0f), Color.green);

            DrawUnderbar(GUIBarType.UnderbarRight,
                new Vector2(Screen.width / 2.0f + offset, Screen.height / 2.0f) +
                new Vector2(22.0f, 3.0f - GUIExtensions.texture_bar_mask_left.height / 2.0f),
                "Hull:", stats.hull, stats.maxHull, false);
            
            // draw visible targets
            {
                WorldControllerClient world = _target.worldController as WorldControllerClient;

                if (world != null)
                {
                    // guranteed to have center of components' hull to be inside camera view
                    List<WorldObjectComponent> visibleComponents = world.visibleComponents;

                    bool targetWasSet = false;

                    foreach (WorldObjectComponent e in visibleComponents)
                    {
                        // skip player ship
                        if (world.clientWorldObject == e.parentWorldObject.id) continue;
                        if (selectedComponent == e) continue;
                        if (e.hull == null) continue;
                        if (Helpers.FastLength(e.hull.hullGlobalPosition - _target.rootHullPosition) > stats.radarRange) continue;

                        Vector3 proj = world.systemCamera.camera.WorldToScreenPoint(e.hull.hullGlobalPosition);
                        if (proj.z < 0.0f) continue;

                        DrawTargetSimple(e, proj, world.mouseSelectedComponent == e);

                        // set target too
                        {
                            if (Input.GetMouseButtonDown(0) && (_target != e.parentWorldObject) && !targetWasSet)
                            {
                                Vector2 a = Input.mousePosition;
                                Vector2 b = proj;
                                if ((a - b).sqrMagnitude <= (16.0f * 16.0f))
                                {
                                    if (selectedComponent != e)
                                    {
                                        selectedComponent = e;
                                        selectedComponent.parentWorldObject.AddDelegate(this);
                                        targetWasSet = true;
                                    }
                                }
                            }
                        }
                    }

                    // set target
                    if (Input.GetMouseButtonDown(0) && !targetWasSet)
                    {
                        if (world.mouseSelectedComponent != null &&
                            world.mouseSelectedComponent.parentWorldObject != _target &&
                            selectedComponent != world.mouseSelectedComponent )
                        {
                            selectedComponent = world.mouseSelectedComponent;
                            selectedComponent.parentWorldObject.AddDelegate(this);
                        }

                    }

                    if (selectedComponent != null)
                    {
                        Vector3 targetHullPos = selectedComponent.hull.hullGlobalPosition;
                        Vector3 proj = world.systemCamera.camera.WorldToScreenPoint(targetHullPos);
                        if (proj.z > 0.0f)
                        {
                            Vector2 center = new Vector2(proj.x, Screen.height - proj.y);

                            GUI.DrawTexture(
        Helpers.MakeRect(center, GUIExtensions.texture_target_box_big.width, GUIExtensions.texture_target_box_big.height),
        GUIExtensions.texture_target_box_big);

                            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                            ShadowedLabel.Draw(Helpers.MakeRect(center + new Vector2(0.0f, -GUIExtensions.texture_target_box_big.height / 2.0f), 200.0f, 30.0f), selectedComponent.parentWorldObject.hudTargetObjectName);

                            float dist = Helpers.FastLength(selectedComponent.hull.hullGlobalPosition - _target.rootHullPosition);
                            ShadowedLabel.Draw(Helpers.MakeRect(center + new Vector2(0.0f, +GUIExtensions.texture_target_box_big.height / 2.0f), 200.0f, 30.0f), (int)dist + "m");

                            // draw target cross
                            {
                                Vector3 targetVel = selectedComponent.hull.velocity;
                                Vector3 vel = _target.root.hullVelocity;
                                Vector3 pos = _target.rootHullPosition;
                                Vector3 lead = Helpers.MakeLead(pos, vel + (targetHullPos - pos).normalized * _target.averageWeaponSpeed, targetHullPos, targetVel);
                                Vector3 projLead = world.systemCamera.camera.WorldToScreenPoint(lead);
                                Vector2 screenLead = new Vector2(projLead.x, Screen.height - projLead.y);
                                Helpers.DrawTexture(screenLead, GUIExtensions.texture_target_cross);

                                {
                                    Vector2 mpos = Input.mousePosition;
                                    Vector2 projLead2 = projLead;

                                    float targetRadius = 10.0f;
                                    if ((mpos - projLead2).sqrMagnitude <= (targetRadius * targetRadius))
                                    {
                                        _target.controlPanel.weaponAimPoint = lead;
                                        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                                        ShadowedLabel.Draw(Helpers.MakeRect(center + new Vector2(0.0f, -GUIExtensions.texture_target_box_big.height / 2.0f - 20.0f), 200.0f, 30.0f), "Locked");

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        

        void DrawSelectedTargetBox()
        {
        }

        void DrawTargetSimple(WorldObjectComponent e, Vector3 pos, bool mouseOver)
        {
            if (Event.current.type != EventType.Repaint) return;

            Vector2 center = new Vector2(pos.x, Screen.height - pos.y);

            GUI.color = Color.white;

            // Do not draw selector on static objects
            if (e.parentWorldObject.type != WorldObjectType.Static)
            {
                if (!mouseOver)
                {
                    GUI.DrawTexture(
                        Helpers.MakeRect(center, GUIExtensions.texture_target_box_small.width, GUIExtensions.texture_target_box_small.height),
                        GUIExtensions.texture_target_box_small);
                }
                else
                {
                    float overSize = 1.1f;
                    GUI.DrawTexture(
                    Helpers.MakeRect(center, GUIExtensions.texture_target_box_small.width * overSize, GUIExtensions.texture_target_box_small.height * overSize),
                    GUIExtensions.texture_target_box_small);
                }

                // draw name
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                ShadowedLabel.Draw(Helpers.MakeRect(center + new Vector2(0.0f, -GUIExtensions.texture_target_box_small.height), 200.0f, 30.0f), e.parentWorldObject.hudTargetObjectName);
            }

        }

        enum GUIBarType
        {
            Left, Right, UnderbarLeft, UnderbarRight
        }

        void DrawUnderbar(GUIBarType type, Vector2 pos, string title, float current, float max, bool drawMax)
        {
            if (Event.current.type != EventType.Repaint) return;

            if (type == GUIBarType.UnderbarLeft)
            {
                Rect rect = Helpers.MakeRect(pos, GUIExtensions.texture_underbar_left.width, GUIExtensions.texture_underbar_left.height);
                GUI.DrawTexture(rect, GUIExtensions.texture_underbar_left);
                
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                float labelHeight = 40.0f;
                Vector2 labelOffset = new Vector2(-GUIExtensions.texture_underbar_left.width/2.0f, -labelHeight / 2.0f);
                rect = Helpers.MakeRect(pos + labelOffset, GUIExtensions.texture_underbar_left.width, labelHeight);
                string txt;

                if (drawMax)
                    txt = title + "\n" + (int)current + " / " + (int)max;
                else
                    txt = title + "\n" + (int)current;

                ShadowedLabel.Draw(rect, txt);
                return;
            }

            if (type == GUIBarType.UnderbarRight)
            {
                Rect rect = Helpers.MakeRect(pos, GUIExtensions.texture_underbar_right.width, GUIExtensions.texture_underbar_right.height);
                GUI.DrawTexture(rect, GUIExtensions.texture_underbar_right);

                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                float labelHeight = 40.0f;
                Vector2 labelOffset = new Vector2(GUIExtensions.texture_underbar_right.width / 2.0f, -labelHeight / 2.0f);
                rect = Helpers.MakeRect(pos + labelOffset, GUIExtensions.texture_underbar_right.width, labelHeight);
                string txt;

                if (drawMax)
                    txt = title + "\n" + (int)current + " / " + (int)max;
                else
                    txt = title + "\n" + (int)current;

                ShadowedLabel.Draw(rect, txt);
                return;
            }
        }

        void GLDrawTexture(Rect rect, Texture tex, Material mat)
        {
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.LoadPixelMatrix();
            mat.mainTexture = tex;
            mat.SetPass(0);
            
            GL.Begin(GL.TRIANGLES);

            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(rect.xMin, rect.yMin, 0);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(rect.xMin, rect.yMax, 0);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(rect.xMax, rect.yMax, 0);

            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(rect.xMin, rect.yMin, 0);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(rect.xMax, rect.yMin, 0);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(rect.xMax, rect.yMax, 0);

            GL.End();
            GL.PopMatrix();
        }

        void DrawBar(GUIBarType type, float value, float max, Vector2 pos, Color color)
        {
            if (Event.current.type != EventType.Repaint) return;

            if( type == GUIBarType.Left )
            {
                Rect rect = Helpers.MakeRect(pos, GUIExtensions.texture_bar_mask_left.width, GUIExtensions.texture_bar_mask_left.height);
                Color c = color;
                c.a = 0.2f;
                GUIExtensions.material_gui_bar.color = c;
                GUIExtensions.material_gui_bar.SetFloat("_BarValue", 1.0f);
                GLDrawTexture(rect, GUIExtensions.texture_bar_mask_left, GUIExtensions.material_gui_bar);

                c.a = 0.4f;
                GUIExtensions.material_gui_bar.color = c;
                GUIExtensions.material_gui_bar.SetFloat("_BarValue", value / max);
                GLDrawTexture(rect, GUIExtensions.texture_bar_mask_left, GUIExtensions.material_gui_bar);
                return;
            }

            if (type == GUIBarType.Right)
            {
                Rect rect = Helpers.MakeRect(pos, GUIExtensions.texture_bar_mask_right.width, GUIExtensions.texture_bar_mask_right.height);
                Color c = color;
                c.a = 0.2f;

                GUIExtensions.material_gui_bar.color = c;
                GUIExtensions.material_gui_bar.SetFloat("_BarValue", 1.0f);
                GLDrawTexture(rect, GUIExtensions.texture_bar_mask_right, GUIExtensions.material_gui_bar);

                c.a = 0.4f;
                GUIExtensions.material_gui_bar.color = c;
                GUIExtensions.material_gui_bar.SetFloat("_BarValue", value / max);
                GLDrawTexture(rect, GUIExtensions.texture_bar_mask_right, GUIExtensions.material_gui_bar);
                return;
            }
        }

        #region IWorldObjectDelegate Members

        public void OnWorldObjectReleased(WorldObject obj)
        {
            if (selectedComponent != null && selectedComponent.parentWorldObject == obj)
            {
                selectedComponent = null;
            }

            if (_target == obj) _target = null;
        }

        #endregion
    }
}
