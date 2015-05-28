using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeUniverse.Core;

namespace FreeUniverse.ViewControllers
{
    public class ViewControllerConsole : ViewControllerBase
    {
        public static ViewControllerConsole instance;

        public ViewControllerConsole()
        {  
            instance = this;
            visible = false;
            ViewControllerManager.Remove(this); //since base constructor adds
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        List<string> _strings = new List<string>();

        public void Log(string str, bool post)
        {
            _strings.Insert(0, str);

            // Post notification
            if( post )
            NotificationCenter.Post(NotificationID.ConsoleInput, str);
        }

        public void Clear()
        {
            _strings.Clear();
        }

        //Rect boxRect = Rect.MinMaxRect(0.0f, 0.0f, Screen.width, Screen.height/2.0f);
        const float STRING_HEIGHT = 15.0f;
        const float LEFT_INDENT = 5.0f;
        const float TOP_OFFSET = -4.0f;
        //const float DRAW_HEIGHT = 
        string _input = "";
        bool _inputSelected = false;
        static string INPUT_CONTROL_NAME = "console_input";

        public override void OnGUI(Event e)
        {
            GUI.skin = GUIExtensions.SKIN_DEFAULT;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;

            base.OnGUI(e);

            if ((e.type == EventType.KeyUp ) && (e.keyCode == KeyCode.F1))
                visible = !visible;

            if (visible == false)
                return;

            // Draw background box
            GUI.Box(Rect.MinMaxRect(0.0f, 0.0f, Screen.width, Screen.height / 2.0f), "");

            // Draw strings
            int renderCount = (int)((Screen.height / 2.0f) / STRING_HEIGHT);
            int i = 0;

            float draw_height = Screen.height/2.0f ;
            
            foreach (string s in _strings)
            {
                if (i >= renderCount)
                    break;

                GUI.Label(Rect.MinMaxRect(LEFT_INDENT, (draw_height) - STRING_HEIGHT - STRING_HEIGHT * (i + 1) - STRING_HEIGHT + TOP_OFFSET, Screen.width, (draw_height) - STRING_HEIGHT - STRING_HEIGHT - STRING_HEIGHT * (i)), s);

                i++;
            }

            // draw input

            if (e.type == EventType.KeyDown && (e.keyCode == KeyCode.Return) && _inputSelected)
            {
                Log(_input, true);
                _input = "";
            }
            else
            {
                GUI.SetNextControlName(INPUT_CONTROL_NAME);
                _input = GUI.TextField(Rect.MinMaxRect(0.0f, (draw_height) - STRING_HEIGHT + TOP_OFFSET, Screen.width, (draw_height)), _input);
                _inputSelected = GUI.GetNameOfFocusedControl().CompareTo(INPUT_CONTROL_NAME) == 0;
            }
        }
    }
}
