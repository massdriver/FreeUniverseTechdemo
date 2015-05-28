using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.ViewControllers
{
    public class ViewControllerManager : BaseGameObject
    {
        List<ViewControllerBase> _viewControllers = new List<ViewControllerBase>();

        static ViewControllerManager instance;

        public ViewControllerManager()
        {
            instance = this;
            gameObject.name = "View Controller Manager";
        }

        public static void Add(ViewControllerBase controller)
        {
            instance._viewControllers.Add(controller);
        }

        public static void Remove(ViewControllerBase controller)
        {
            instance._viewControllers.Remove(controller);
        }

        public override void OnGUI()
        {
            Event e = Event.current;

            if (PopupMessageBox.OnGUI())
                return;

            foreach (ViewControllerBase c in _viewControllers)
                c.OnGUI(e);

            if( ViewControllerConsole.instance != null)
                ViewControllerConsole.instance.OnGUI(e);
        }

        public override void OnUpdate()
        {
            foreach (ViewControllerBase c in _viewControllers)
                c.OnUpdate();

            if (ViewControllerConsole.instance != null)
                ViewControllerConsole.instance.OnUpdate();
        }
    }
}
