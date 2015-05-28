using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.GUILib
{
    public interface ITableViewDelegate
    {
        void OnTableViewItemSelected(TableView table, int item); 
    }

    public class TableView : UIView
    {
        public Rect area { get; set; }
        public ITableViewDelegate tableViewDelegate { get; set; }
        public int selectedRow { get; set; }
        public Vector2 scroll { get; set; }
        private List<TableItem> items { get; set; }

        private struct TableItem
        {
            public string item;
            public TableItem(string str){ item = str; }
        }

        public TableView(Rect area)
        {
            this.area = area;
            items = new List<TableItem>();
            useAreaRect = false;
        }

        public void AddItem(string item)
        {
            items.Add(new TableItem(item));
        }

        public bool useAreaRect { get; set; }

        override public void OnGUI()
        {
            GUI.skin = GUIExtensions.SKIN_DEFAULT;

            if( useAreaRect )
                GUILayout.BeginArea(area);

            scroll = GUILayout.BeginScrollView(scroll);

            Color back = GUI.backgroundColor;

            int i = 0;
            foreach (TableItem e in items)
            {
                if (i == selectedRow)
                    GUI.backgroundColor = Color.red;
                else
                    GUI.backgroundColor = back;

                if (GUILayout.Button(e.item))
                {
                    selectedRow = i;

                    if (tableViewDelegate != null)
                        tableViewDelegate.OnTableViewItemSelected(this, i);
                }

                i++;
            }

            GUILayout.EndScrollView();

            if( useAreaRect )
                GUILayout.EndArea();

            GUI.backgroundColor = back;
        }
    }
}
