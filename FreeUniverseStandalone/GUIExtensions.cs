using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse
{
    public class GUIExtensions
    {
        public static Texture2D HUD_BAR_ELEMENT_SPEED;

        public static Texture2D BAR_TEXTURE_LEFT;
        public static Texture2D BAR_TEXTURE_RIGHT;
        public static GUISkin SKIN_DEFAULT;
        public static Material DEFAULT_LINE_MATERIAL;
        public static GameObject hud_bar_left_mesh;
        public static Texture2D texture_bar_mask_left;
        public static Texture2D texture_bar_mask_right;
        public static Material material_gui_bar;


        public static Texture2D texture_underbar_right;
        public static Texture2D texture_underbar_left;
        public static Texture2D texture_target_box_small;
        public static Texture2D texture_target_box_big;
        public static Texture2D texture_target_cross;

        public static void Init()
        {
            //hud_bar_left_mesh = Helpers.LoadUnityObject("GUI/hud_bar_left_mesh");
            //hud_bar_left_mesh.renderer.enabled = false;

            HUD_BAR_ELEMENT_SPEED = (Texture2D)Resources.Load("GUI/hud_bar_element", typeof(Texture2D));
            SKIN_DEFAULT = (GUISkin)Resources.Load("skin_freeuniverse", typeof(GUISkin)); 
            BAR_TEXTURE_LEFT = (Texture2D)Resources.Load("GUI/hud_bar_left", typeof(Texture2D));
            BAR_TEXTURE_RIGHT = (Texture2D)Resources.Load("GUI/hud_bar_right", typeof(Texture2D));

            DEFAULT_LINE_MATERIAL = new Material("Shader \"Lines/Colored Blended\" {" +
            "SubShader { Pass { " +
            "    Blend SrcAlpha OneMinusSrcAlpha " +
            "    ZWrite Off Cull Off Fog { Mode Off } " +
            "    BindChannels {" +
            "      Bind \"vertex\", vertex Bind \"color\", color }" +
            "} } }");
            DEFAULT_LINE_MATERIAL.hideFlags = HideFlags.HideAndDontSave;
            DEFAULT_LINE_MATERIAL.shader.hideFlags = HideFlags.HideAndDontSave;

            texture_target_cross = Resources.Load("GUI/target_cross") as Texture2D;
            texture_target_box_big = Resources.Load("GUI/target_box_big") as Texture2D;
            texture_target_box_small = Resources.Load("GUI/target_box_small") as Texture2D;
            texture_underbar_right = Resources.Load("GUI/hud_underbar_right") as Texture2D;
            texture_underbar_left = Resources.Load("GUI/hud_underbar_left") as Texture2D;
            texture_bar_mask_left = Resources.Load("GUI/hud_bar_mask_left") as Texture2D;
            texture_bar_mask_right = Resources.Load("GUI/hud_bar_mask_right") as Texture2D;
            material_gui_bar = Resources.Load("GUI/material_gui_bar") as Material;
        }
    }
}
