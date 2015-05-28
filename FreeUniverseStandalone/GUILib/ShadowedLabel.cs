using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.GUILib
{
    public class ShadowedLabel
    {
        public static void Draw(Rect rect, string text, Color shadowColor)
        {
            Color saveColor = GUI.contentColor;
            Rect shadowRect = rect;
            GUI.contentColor = shadowColor;

            shadowRect.x--;
            GUI.Label(shadowRect, text);
            shadowRect.x += 2;
            GUI.Label(shadowRect, text);

            shadowRect.x -= 1;
            shadowRect.y += 1;
            GUI.Label(shadowRect, text);
            shadowRect.y -= 2;
            GUI.Label(shadowRect, text);

            GUI.contentColor = saveColor;
            GUI.Label(rect, text);
        }

        public static void Draw(Rect rect, string text)
        {
            Draw(rect, text, Color.black);
        }
    }
}
