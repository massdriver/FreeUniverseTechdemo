using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.GUILib
{
    public class DropDownList
    {
        public bool drop { get; private set; }
        public int count { get; private set; }
        private List<string> items { get; set; }
        public Rect rect { get; set; }
        public bool visible { get; set; }
        public int selected { get; private set; }
        public int maxDropElements { get; set; }
        public int scroll { get; set; }
        private Vector2 internal_scroll { get; set; }

        public DropDownList(Rect rect)
        {
            this.rect = rect;
            this.visible = true;
            this.items = new List<string>();
            maxDropElements = 5;
        }

        public void Add(string item)
        {
            items.Add(item);
            count = items.Count;
        }

        public void OnGUI()
        {
            // draw regular, selected item
            if (items.Count == 0)
            {
                GUILayout.Button("");
                return;
            }

            if (GUILayout.Button(items[selected]))
                drop = !drop;
            
            if (drop)
            {
                Rect listRect = Rect.MinMaxRect(rect.xMin, rect.yMax, rect.xMax, rect.yMax + rect.height * maxDropElements);
                
                internal_scroll = GUILayout.BeginScrollView(internal_scroll);

                int i = 0;
                foreach (string str in items)
                {
                    if (GUILayout.Button(str))
                    {
                        selected = i;
                        drop = false;
                    }

                    i++;
                }

                GUILayout.EndScrollView();
                //GUILayout.EndArea();
            }
           
        }

    }
}
