using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.ViewControllers
{
    public class PopupMessageBox
    {
        private class PopupMessage
        {
            public PopupMessage(string cap, string msg) { caption = cap; message = msg; }
            public string caption;
            public string message;
        }

        private static List<PopupMessage> messages = new List<PopupMessage>();
        private static PopupMessage currentMessage = null;

        public static void Show(string caption, string text)
        {
            messages.Add(new PopupMessage(caption, text));
        }

        public const float WindowWidth = 450.0f;
        public const float WindowHeight = 85.0f;

        public static bool OnGUI()
        {
            if (currentMessage == null && messages.Count > 0)
            {
                currentMessage = messages[0];
                messages.RemoveAt(0);
            }

            if (currentMessage != null)
            {
                Rect area = Helpers.MakeRectCentered(WindowWidth, WindowHeight);

                GUI.Window((int)GUIWindowTypes.PopupMessageBox, area, WndProc, currentMessage.caption);

                return true;
            }

            return false;
        }

        private static void WndProc(int id)
        {
            GUI.skin = GUIExtensions.SKIN_DEFAULT;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;

            GUILayout.BeginArea(Rect.MinMaxRect(
                GUIStaticData.WindowHorizontalOffset,
                GUIStaticData.WindowVerticalOffset,
                WindowWidth - GUIStaticData.WindowHorizontalOffset,
                WindowHeight - GUIStaticData.WindowVerticalOffset / 2.0f));


            GUILayout.Label(currentMessage.message);

            GUILayout.BeginHorizontal("");

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Ok", GUILayout.MaxWidth(100.0f)))
                currentMessage = null;

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }
}
