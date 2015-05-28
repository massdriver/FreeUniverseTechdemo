using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.GUILib
{
    public class UIView
    {
        public UIView superView { get; private set; }
        public object userData { get; set; }
        public string name { get; set; }
        public Vector2 center { get; set; }
        public Rect rect { get; set; }
        private List<UIView> subViews { get; set; }

        public UIView()
        {

        }

        public virtual void Render()
        {
            RenderSelf();

            foreach (UIView v in subViews)
                v.Render();
        }

        public virtual void RenderSelf()
        {

        }

        public virtual void OnGUI()
        {
        }
    }
}
