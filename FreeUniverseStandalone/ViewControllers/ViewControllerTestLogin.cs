using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeUniverse.GUILib;
using FreeUniverse.Arch;

namespace FreeUniverse.ViewControllers
{
    public interface ITestLoginViewControllerDelegate
    {
        void OnTestLoginViewControllerActionLogin(string player, string ip);
    }

    public class ViewControllerTestLogin : ViewControllerBase
    {
        private const float VIEW_WIDTH = 180.0f;
        private const float VIEW_HEIGHT = 205.0f;
        private const float QUIT_BUTTON_HEIGHT = 50.0f;
        private const float VIEW_HEIGHT_INDENT = 10.0f;
        private const float VIEW_WIDTH_INDENT = 10.0f;
        private Rect _wndRect = Helpers.MakeRect(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f), VIEW_WIDTH, VIEW_HEIGHT);

        public ITestLoginViewControllerDelegate controllerDelegate { get; set; }

        public override void OnGUI(UnityEngine.Event e)
        {
            if (!visible) return;

            base.OnGUI(e);

            UnityEngine.GUI.Window(GUIStaticData.WND_TEST_LOGIN_VIEW, _wndRect, WindowFunctionLogin, "Login");
        }

        public string playerNickname { get; set; }
        public string serverIP { get; private set; }
        private DropDownList templateList { get; set; }
        public ulong startingTemplate { get; private set; }

        private int itemid;
        private string itemname;
        private int maxitems;

        void SetNextItem()
        {
            itemid++;

            if (itemid >= maxitems) itemid = 0;

            int id = 0;
            foreach (KeyValuePair<ulong, PlayerStartTemplate> t in PlayerStartTemplate.templates)
            {
                if (id == itemid)
                {
                    startingTemplate = t.Key;
                    itemname = t.Value.nickname;
                    break;
                }

                id++;
            }
        }

        public ViewControllerTestLogin()
        {
            visible = false;
            playerNickname = "player";
            serverIP = "127.0.0.1";
            templateList = new DropDownList(Helpers.MakeRect(new Vector2(VIEW_WIDTH / 2.0f, VIEW_HEIGHT - 32.0f), VIEW_WIDTH, 30.0f));

            itemid = -1;
            maxitems = PlayerStartTemplate.templates.Count;
            SetNextItem();
        }

        void WindowFunctionLogin(int id)
        {
            
            GUI.skin = GUIExtensions.SKIN_DEFAULT;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;

            GUILayout.BeginArea(Rect.MinMaxRect(VIEW_WIDTH_INDENT, 5.0f + VIEW_HEIGHT_INDENT, VIEW_WIDTH - VIEW_WIDTH_INDENT, VIEW_HEIGHT - VIEW_HEIGHT_INDENT));

            GUILayout.Label("Nickname:");
            playerNickname = GUILayout.TextField(playerNickname, 32);
            
            GUILayout.Label("Server IP:");
            serverIP = GUILayout.TextField(serverIP, 32);

            if (GUILayout.Button("Login"))
            {
                if (controllerDelegate != null) controllerDelegate.OnTestLoginViewControllerActionLogin(playerNickname, serverIP);
            }

            GUILayout.Label("Ship:");

            if (GUILayout.Button(itemname))
                SetNextItem();
            
            GUILayout.EndArea();
            UnityEngine.GUI.DragWindow();
        }
    }
}
