using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeUniverse.GUILib;

namespace FreeUniverse.ViewControllers
{
    public class ViewControllerBase
    {
        bool _visible = false;

        public virtual bool visible { get { return _visible; } set { _visible = value; } }

        protected List<UIView> subViews { get; set; }

        public virtual void Show()
        {
            visible = true;
        }

        public virtual void Hide()
        {
            visible = false;
        }

        public ViewControllerBase()
        {
            ViewControllerManager.Add(this);
            subViews = new List<UIView>();
        }

        ~ViewControllerBase()
        {
            ViewControllerManager.Remove(this);
        }

        public virtual void OnGUI(Event e)
        {

        }

        public virtual void OnUpdate()
        {

        }
    }
}
